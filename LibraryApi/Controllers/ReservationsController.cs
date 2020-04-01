using LibraryApi.Domain;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class ReservationsController : Controller
    {
        LibraryDataContext Context;

        public ReservationsController(LibraryDataContext context)
        {
            Context = context;
        }

        [HttpPost("/reservations")]
        public async Task<ActionResult> AddReservation([FromBody] PostReservationRequest request)
        {
            // validate
            // add it to the database
            var reservation = new Reservation
            {
                For = request.For,
                Books = string.Join(',', request.Books),
                ReservationCreated = DateTime.Now,
                Status = ReservationStatus.Pending
            };
            Context.Reservations.Add(reservation);
            await Context.SaveChangesAsync();
            // write a message to the queue
            // TODO: RabbitMQ
            // return a response (201) 
            return Ok(reservation);
        }
       
    }
}
