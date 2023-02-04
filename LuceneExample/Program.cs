using System;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace LuceneExample
{
    class Program
    { 
        static void Main(string[] args)
        {
            WriteDocument();
            SearchSomething();

            Console.ReadLine();
        }


        private static void WriteDocument()
        {
            const string indexDirPath = "LuceneIndex";

            var indexDirectory = new DirectoryInfo(indexDirPath);
            if (indexDirectory.Exists)
            {
                indexDirectory.Delete(true);
            }

            var directory = FSDirectory.Open(new DirectoryInfo(indexDirPath));

            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
            var writer = new IndexWriter(directory, analyzer,
                                         IndexWriter.MaxFieldLength.UNLIMITED);

            var document = new Document();

            document.Add(new Field("id", "1", Field.Store.YES, Field.Index.NO));

            document.Add(new Field("postBody", "Lorem ipsum", Field.Store.YES,
                               Field.Index.ANALYZED));

            writer.AddDocument(document);

            writer.Optimize();
            writer.Commit();
            writer.Close();
        }


        private static void SearchSomething()
        {
            var directory = FSDirectory.Open(new DirectoryInfo("LuceneIndex"));

            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);

            var parser = new QueryParser(Version.LUCENE_29, "postBody", analyzer);
            Query query = parser.Parse("lorem*");

            var searcher = new IndexSearcher(directory, true);

            TopDocs topDocs = searcher.Search(query, 10);

            int results = topDocs.ScoreDocs.Length;
            Console.WriteLine("Found {0} results", results);

            for (int i = 0; i < results; i++)
            {
                ScoreDoc scoreDoc = topDocs.ScoreDocs[i];
                float score = scoreDoc.Score;
                int docId = scoreDoc.Doc;
                Document doc = searcher.Doc(docId);

                Console.WriteLine("Result num {0}, score {1}", i + 1, score);
                Console.WriteLine("ID: {0}", doc.Get("id"));
                Console.WriteLine("Text found: {0}\r\n", doc.Get("postBody"));
            }

            searcher.Close();
            directory.Close();
        }

    }
}
