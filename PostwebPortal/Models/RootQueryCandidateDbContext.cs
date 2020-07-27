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
                .HasKey(o => new { o.queryid });
        }
        public DbSet<RootQueryCandidate> url_pwqueries_selection { get; set; }
        //public DbSet<QueryUrlInfo> answerid_pwqueries_selection { get; set; }
        public List<RootQueryCandidate> getQueryCandidates() => url_pwqueries_selection.Local.ToList<RootQueryCandidate>();
        public List<RootQueryCandidate> getQueryCandidatesByAnswerId(string AnswerId) => url_pwqueries_selection.Where(q => q.selectedanswerid == AnswerId).OrderByDescending(i => i.impression).ToList();
        public List<RootQueryCandidate> getQueryCandidatesByUrl(string Url) => url_pwqueries_selection.Where(q => q.url == Url).OrderByDescending(i => i.impression).ToList();

        public void LoadTest()
        {
            url_pwqueries_selection.Add(new RootQueryCandidate { selectedanswerid="a",impression=1,lastmodifiedby="",pwquery="test1",lastmodifiedon=DateTime.Now});
            url_pwqueries_selection.Add(new RootQueryCandidate { selectedanswerid = "b", impression = 2, lastmodifiedby = "", pwquery = "test2", lastmodifiedon = DateTime.Now });
        }
    }
}
