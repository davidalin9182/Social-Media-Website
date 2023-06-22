using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Hosting;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Social_Media_Website.Helpers
{
    public class SearchIndexer
    {
        private readonly Lucene.Net.Store.Directory _directory;
        private readonly IUserRepository _userRepository;

        public SearchIndexer(IWebHostEnvironment env, IUserRepository productRepository)
        {
            var indexPath = Path.Combine(env.WebRootPath, "search_index_directory");
            _directory = new SimpleFSDirectory(new DirectoryInfo(indexPath));
            _userRepository = productRepository;


            if (!IndexReader.IndexExists(_directory))
            {
                CreateIndex();
            }
        }

        public void CreateIndex()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var indexWriter = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            var users = _userRepository.GetAllUsers().Result;

            foreach (var user in users)
            {
                var doc = new Document();
                doc.Add(new Field("Id", user.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("UserName", user.Name, Field.Store.YES, Field.Index.ANALYZED));
                indexWriter.AddDocument(doc);
            }

            indexWriter.Optimize();
            indexWriter.Dispose();
        }

        public IEnumerable<AppUser> Search(string searchTerm)
        {
            using (var searcher = new IndexSearcher(_directory, true))
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "UserName" }, analyzer);
                var query = parser.Parse(searchTerm);

                var hits = searcher.Search(query, null, 1000).ScoreDocs;


                var userIds = hits.Select(hit => searcher.Doc(hit.Doc).Get("Id"));

                var users = _userRepository.GetByIdsAsync(userIds).Result;

                return users;
            }
        }


        public void UpdateUserProfile(string userId, string newUserName)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            var term = new Term("Id", userId);
            var doc = new Document();
            doc.Add(new Field("Id", userId, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("UserName", newUserName, Field.Store.YES, Field.Index.ANALYZED));


            writer.UpdateDocument(term, doc);
            writer.Optimize();
            writer.Dispose();
        }




    }

}



