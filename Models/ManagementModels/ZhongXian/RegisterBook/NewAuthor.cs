using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace test2.Models.ManagementModels.ZhongXian.RegisterBook
{
    public class NewAuthor
    {
        private readonly Test2Context _context;
        public NewAuthor(Test2Context context)
        {
            _context = context;
        }
        // 建立新作者
        public async Task<int> CreateAuthor(int AuthorId, string AuthorName)
        {
            Debug.WriteLine($"++++++++++++++{AuthorId} ++++ {AuthorName} ");
            var authoTest = await _context.Authors.FirstOrDefaultAsync(x => x.AuthorId == AuthorId && x.Author1 == AuthorName);
            if (authoTest != null) { return AuthorId; }

            var newAuthor = new Author { Author1 = AuthorName };
            _context.Authors.Add(newAuthor);
            _context.SaveChanges();
            var authoerId = newAuthor.AuthorId;
            return authoerId;
        }
    }
}
