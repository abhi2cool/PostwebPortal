using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PostwebPortal.Models
{
    public class RootQueryCandidateDbContext : DbContext
    {
        public RootQueryCandidateDbContext([NotNullAttribute] DbContextOptions<RootQueryCandidateDbContext> options) : base(options)
        {
            //LoadTest();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RootQueryCandidate>()
                .HasKey(o => new { o.answerid, o.pwquery });
        }
        public DbSet<RootQueryCandidate> answerid_pwqueries_selection { get; set; }
        public List<RootQueryCandidate> getQueryCandidates() => answerid_pwqueries_selection.Local.ToList<RootQueryCandidate>();
        public List<RootQueryCandidate> getQueryCandidatesByAnswerId(string AnswerId) => answerid_pwqueries_selection.Where(q => q.answerid == AnswerId).OrderByDescending(i => i.impression).ToList();

        public void LoadTest()
        {
            answerid_pwqueries_selection.Add(new RootQueryCandidate { answerid="a",impression=1,judgedetails="",pwquery="test1",selected="false"});
            answerid_pwqueries_selection.Add(new RootQueryCandidate { answerid = "b", impression = 2, judgedetails = "", pwquery = "test2", selected = "false" });
        }
    }
}
