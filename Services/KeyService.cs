using System;
using Dolarium.Data;
using Dolarium.Models;

namespace Dolarium.Services
{
    public class KeyService
    {
        private readonly AppDBContext _context;

        public KeyService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<Keys> CreateKeyAsync(int limit)
        {
            var key = new Keys
            {
                Id = Guid.NewGuid().ToString(),
                Limit = limit,
                Used = 0,
                CreatedAt = (int)MathF.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds()),
            };

            _context.Keys.Add(key);
            await _context.SaveChangesAsync();
            return key;
        }

        public async Task<Keys> GetKeyAsync(string id)
        {
            return await _context.Keys.FindAsync(id);
        }

        public async Task<bool> DeleteKeyAsync(string id)
        {
            var key = await GetKeyAsync(id);

            if (key == null)
            {
                return false;
            }

            _context.Keys.Remove(key);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IncrementKeyUsageAsync(string id)
        {
            var key = await GetKeyAsync(id);

            if (key == null || key.Used >= key.Limit)
            {
                return false;
            }

            key.Used++;
            _context.Keys.Update(key);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsKeyValidAsync(string id)
        {
            var key = await GetKeyAsync(id);
            return key != null && key.Used < key.Limit;
        }
    }
}
