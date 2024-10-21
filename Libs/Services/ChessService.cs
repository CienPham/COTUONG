using Libs.Entity;
using Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Services
{
    public class ChessService
    {
        private ApplicationDbContext dbContext;
        private RoomRepository roomRepository;

        public ChessService(ApplicationDbContext dbContext) { 
            this.dbContext = dbContext;
            this.roomRepository = new RoomRepository(dbContext);

        }
        public void Save() { 
            dbContext.SaveChanges();
        }
        public List<Room> getRoomList() { 
            return roomRepository.getRoomList();
        }
        public void insertRoom(Room r) {
            dbContext.Room.Add(r);
            Save();
        }
    }
}
