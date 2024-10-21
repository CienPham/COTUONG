using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BanCov2.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private ChessService chessService;

        public RoomController(ChessService chessService)
        {
            this.chessService = chessService;
        }
        [HttpGet("get-room")]
        public ActionResult getRoomList() { 
            List<Room> roomList = chessService.getRoomList();
            return Ok( new { status = true, message = "", data = roomList });
        }
        [HttpPost("insert-room")]
        public ActionResult insertRoom(Room room)
        {
            Room r = new Room();
            r.Id = Guid.NewGuid();
            r.Name = room.Name;
            chessService.insertRoom(r);
            return Ok(new { status = true, message = "" });
        }

    }
}
