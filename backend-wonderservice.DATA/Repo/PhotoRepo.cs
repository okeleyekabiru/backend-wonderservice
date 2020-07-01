using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.EntityFrameworkCore;
using WonderService.Data.Services;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.DATA.Repo
{
  public class PhotoRepo : IPhoto
  {
    private readonly DataContext _context;

    public PhotoRepo(DataContext context)
    {
      _context = context;
    }

    public async Task Add(Photo photo)
    {
      await _context.AddAsync(photo);

    }

    public async Task AddRange(List<Photo> photo)
    {
      await _context.AddRangeAsync(photo);


    }

    public async Task<Photo> Get(string id)
    {
      return await _context.Photo.Where(r => r.Id.Equals(Guid.Parse(id))).FirstOrDefaultAsync();
    }

    public Task Delete(Photo photo)
    {
      _context.Remove(photo);
      return Task.CompletedTask;

    }

    public async Task<ReturnResult> SaveChanges()
    {
      if (await _context.SaveChangesAsync() > 0)
        return new ReturnResult
        {
          Succeeded = true
        };

      return new ReturnResult
      {
        Error = "An error occurred while updating database"
      };
    }
  }
}
