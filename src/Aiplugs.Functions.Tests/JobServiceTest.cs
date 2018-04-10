using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aiplugs.Functions.Core;
using Moq;
using Xunit;

namespace Aiplugs.Functions.Tests
{
    public class JobServiceTest
    {
        public class TestParams {}
        private IJobRepository CreateJobRepository()
        {
            long count = 0;
            var list = new Dictionary<long, IJob>();
            var mock = new Mock<IJobRepository>();
            mock.Setup(m => m.AddAsync(It.IsAny<IJob>())).ReturnsAsync((IJob job) =>
            {
                var id = ++count;
                job.Id = id;
                list.Add(id, job);
                return id;
            });
            mock.Setup(m => m.FindAsync(0)).ReturnsAsync(() => new Job { Name = "Test" });
            mock.Setup(m => m.FindAsync(It.IsAny<long>())).ReturnsAsync((long id) => list[id]);
            mock.Setup(m => m.UpdateAsync(It.IsAny<IJob>())).Returns(() => Task.FromResult(0));

            return mock.Object;
        }
        private IJobRegistory CreateJobRegistory()
        {
            var mock = new Mock<IJobRegistory>();
            mock.Setup(m => m.AddJob(It.IsAny<long>(), It.IsAny<IJob>()));
            return mock.Object;
        }
        private ILockService CreateLockService(bool ret, Action onUnlock = null)
        {
            if (onUnlock == null)
                onUnlock = () => {};
            var mock = new Mock<ILockService>();
            mock.Setup(m => m.LockAsync(It.IsAny<string>())).ReturnsAsync(ret);
            mock.Setup(m => m.UnlockAsync(It.IsAny<string>())).Returns(() => { onUnlock(); return Task.FromResult(0); });
            return mock.Object;
        }
        private IUserResolver CreateUserResolver()
        {
            var mock = new Mock<IUserResolver>();
            mock.Setup(m => m.GetUserId()).Returns("TEST_USER");
            return mock.Object;
        }

        [Fact]
        public async Task ExclusiveCreateAsync_should_return_value_when_lock_is_success()
        {
            var svc = new JobService(CreateJobRepository(), new JobRegistory(), CreateLockService(true), CreateUserResolver());
            var id = await svc.ExclusiveCreateAsync("Test", new TestParams());
            Assert.True(id.HasValue);
        }

        [Fact]
        public async Task ExclusiveCreateAsync_should_return_value_when_lock_is_faild()
        {
            var svc = new JobService(CreateJobRepository(), new JobRegistory(), CreateLockService(false), CreateUserResolver());
            var id = await svc.ExclusiveCreateAsync("Test", new TestParams());
            Assert.False(id.HasValue);
        }

        [Fact]
        public async Task ExclusiveCreateAsync_should_create_job_by_the_user()
        {
            var svc = new JobService(CreateJobRepository(), new JobRegistory(), CreateLockService(true), CreateUserResolver());
            var id = await svc.ExclusiveCreateAsync("Test", new TestParams());

            Assert.True(id.HasValue);

            var job = await svc.FindAsync(id.Value);

            Assert.Equal("TEST_USER", job.CreatedBy);
        }

        [Fact]
        public async Task DequeueAsync_should_return_instance_after_ExclusiveCreateAsync()
        {
            var svc = new JobService(CreateJobRepository(), new JobRegistory(), CreateLockService(true), CreateUserResolver());
            var id = await svc.ExclusiveCreateAsync("Test", new TestParams());
            var job = await svc.DequeueAsync();

            Assert.NotNull(job);
            Assert.Equal(id.Value, job.Id);
        }

        [Fact]
        public async Task RegisterCanceller_should_register_canceller_to_registory()
        {
            var registory = new JobRegistory();
            var svc = new JobService(CreateJobRepository(), registory, CreateLockService(true), CreateUserResolver());
            var id = await svc.ExclusiveCreateAsync("Test", new TestParams());

            svc.RegisterCanceller(id.Value, () => { });
            Assert.True(registory.ExistCanceller(id.Value));
        }

        [Fact]
        public async Task UnregisterCanceller_should_unregister_canceller_to_registory()
        {
            var registory = new JobRegistory();
            var svc = new JobService(CreateJobRepository(), registory, CreateLockService(true), CreateUserResolver());
            var id = await svc.ExclusiveCreateAsync("Test", new TestParams());

            svc.RegisterCanceller(id.Value, () => { });
            Assert.True(registory.ExistCanceller(id.Value));

            svc.UnregisterCanceller(id.Value);
            Assert.False(registory.ExistCanceller(id.Value));
        }

        [Fact]
        public async Task Cancel_should_call_registered_canceller()
        {
            var registory = new JobRegistory();
            var svc = new JobService(CreateJobRepository(), registory, CreateLockService(true), CreateUserResolver());
            var id = await svc.ExclusiveCreateAsync("Test", new TestParams());

            var canceled = false;
            svc.RegisterCanceller(id.Value, () => { canceled = true; });
            Assert.True(registory.ExistCanceller(id.Value));
            
            svc.Cancel(id.Value);

            Assert.True(canceled);
            Assert.False(registory.ExistCanceller(id.Value));
        }

        [Fact]
        public async Task CancelAll_should_call_registered_cancellers()
        {
            var svc = new JobService(CreateJobRepository(), new JobRegistory(), CreateLockService(true), CreateUserResolver());
            var id1 = await svc.ExclusiveCreateAsync("Test", new TestParams());
            var id2 = await svc.ExclusiveCreateAsync("Test", new TestParams());

            var canceled1 = false;
            var canceled2 = false;
            svc.RegisterCanceller(id1.Value, () => { canceled1 = true; });
            svc.RegisterCanceller(id2.Value, () => { canceled2 = true; });
            svc.CancelAll();

            Assert.True(canceled1 && canceled2);
        }

        [Fact]
        public async Task Save_finished_job_should_call_unlock()
        {
            var unlocked = false;
            var registory = new JobRegistory();
            var lockService = CreateLockService(true, () =>
            {
                unlocked = true;
            });
            var svc = new JobService(CreateJobRepository(), registory, lockService, CreateUserResolver());
            var id = await svc.ExclusiveCreateAsync("Test", new TestParams());
            var job = await svc.FindAsync(id.Value);

            job.FinishAt = DateTime.UtcNow;
            await svc.SaveAsync(job);

            Assert.False(registory.ExistJob(id.Value));
            Assert.True(unlocked);
        }
    }
}