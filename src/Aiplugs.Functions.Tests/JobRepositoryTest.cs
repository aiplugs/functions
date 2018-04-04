using System;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.Functions.Core;
using Xunit;

namespace Aiplugs.Functions.Tests
{
    public class JobRepositoryTest
    {
        private async Task All(Func<IJobRepository,Task> action)
        {
            using (var testdb = new TestDb())
            {
                foreach(var db in testdb.DBs)
                {
                    await action(db.JobRepository);
                }
            }
        }

        [Fact]
        public async Task AddAsync_should_return_greeter_than_0()
        {
            await All(async repo => 
            {
                var id = await repo.AddAsync(new Job { Name = "Test" });
                Assert.True(id > 0);
            });
        }

        [Fact]
        public async Task FindAsync_should_return_null_if_not_found()
        {
            await All(async repo => 
            {
                var job = await repo.FindAsync(0);
                Assert.Null(job);
            });
        }

        [Fact]
        public async Task FindAsync_should_return_inserted_by_id_of_inserted()
        {
            await All(async repo => 
            {
                var job = new  Job 
                { 
                    Name = "Test", 
                    CreatedAt = DateTime.Now, 
                    CreatedBy = Guid.Empty.ToString()
                };
                var id = await repo.AddAsync(job);
                var inserted = await repo.FindAsync(id);
                Assert.NotNull(inserted);
                Assert.Equal(job.Name, inserted.Name);
                Assert.Equal(job.CreatedAt, inserted.CreatedAt);
                Assert.Equal(job.CreatedBy, inserted.CreatedBy);
            });
        }

        [Fact]
        public async Task UpdateAsync_should_save_all_properties()
        {
            await All(async repo => 
            {
                var job = new  Job 
                { 
                    Name = "Test", 
                    CreatedAt = DateTime.Now, 
                    CreatedBy = Guid.Empty.ToString()
                };
                job.Id = await repo.AddAsync(job);
                Assert.True(job.Id > 0);

                job.Progress = 100;
                job.Status = JobStatus.Success;
                job.StartAt = DateTime.UtcNow;
                job.FinishAt = DateTime.UtcNow;
                job.Log = "Test";
                await repo.UpdateAsync(job);

                var updated = await repo.FindAsync(job.Id);
                Assert.NotNull(updated);
                Assert.Equal(job.Progress, updated.Progress);
                Assert.Equal(job.Status, updated.Status);
                Assert.Equal(job.StartAt, updated.StartAt);
                Assert.Equal(job.FinishAt, updated.FinishAt);
                Assert.Equal(job.CreatedAt, updated.CreatedAt);
                Assert.Equal(job.CreatedBy, updated.CreatedBy);
                Assert.Equal(job.Log, updated.Log);
            });
        }
        async Task AddJobs(IJobRepository repo)
        {
            await repo.AddAsync(new Job { Name = "Test1" });
            await repo.AddAsync(new Job { Name = "Test1" });
            await repo.AddAsync(new Job { Name = "Test1" });
            await repo.AddAsync(new Job { Name = "Test1" });
            await repo.AddAsync(new Job { Name = "Test1" });
            await repo.AddAsync(new Job { Name = "Test1" });
            await repo.AddAsync(new Job { Name = "Test2" });
            await repo.AddAsync(new Job { Name = "Test2" });
            await repo.AddAsync(new Job { Name = "Test2" });
            await repo.AddAsync(new Job { Name = "Test2" });
            await repo.AddAsync(new Job { Name = "Test3" });
            await repo.AddAsync(new Job { Name = "Test3" });
        }

        [Fact]
        public async Task GetAsync_should_return_amount_of_limit()
        {
            await All(async repo => 
            {
                await AddJobs(repo);

                var jobs = await repo.GetAsync(null, limit: 10);

                Assert.Equal(10, jobs.Count());
            });
        }

        [Fact]
        public async Task GetAsync_should_return_latest_if_desc_is_true()
        {
            await All(async repo => 
            {
                await AddJobs(repo);

                var jobs = await repo.GetAsync(null, desc: true);

                Assert.Equal("Test3", jobs.First().Name);
            });
        }

        [Fact]
        public async Task GetAsync_should_return_oldest_if_desc_is_false()
        {
            await All(async repo => 
            {
                await AddJobs(repo);

                var jobs = await repo.GetAsync(null, desc: false);

                Assert.Equal("Test1", jobs.First().Name);
            });
        }

        [Fact]
        public async Task GetAsync_can_filtering_by_name()
        {
            await All(async repo => 
            {
                await AddJobs(repo);

                var jobs = await repo.GetAsync("Test2");

                Assert.All(jobs, job => 
                {
                    Assert.Equal("Test2", job.Name);
                });
            });
        }

        [Fact]
        public async Task GetAsync_should_return_next_list_if_set_skipToken()
        {
            await All(async repo => 
            {
                await AddJobs(repo);

                var jobs = await repo.GetAsync(null);
                var next = await repo.GetAsync(null, skipToken: jobs.Last().Id);
                var list = jobs.Concat(next);

                var id = list.First().Id;
                foreach(var job in list)
                {
                    Assert.Equal(job.Id, id--);
                }
            });
        }

        [Fact]
        public async Task GetAsync_should_return_next_list_if_set_skipToken_and_desc_is_false()
        {
            await All(async repo => 
            {
                await AddJobs(repo);

                var jobs = await repo.GetAsync(null, desc: false);
                var next = await repo.GetAsync(null, desc: false, skipToken: jobs.Last().Id);
                var list = jobs.Concat(next);

                var id = list.First().Id;
                foreach(var job in list)
                {
                    Assert.Equal(job.Id, id++);
                }
            });
        }
    }
}