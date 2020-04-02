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
        IWriteToTheReservationQueue ReservationQueue;

        public ReservationsController(LibraryDataContext context, IWriteToTheReservationQueue reservationQueue)
        {
            Context = context;
            ReservationQueue = reservationQueue;
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
            // Write the code you wish you had.
            await ReservationQueue.Write(reservation);
            // return a response (201) 
            return Ok(reservation);
        }
       
    }
}
