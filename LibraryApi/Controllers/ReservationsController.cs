using LibraryApi.Domain;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<Reservation>> AddReservation([FromBody] PostReservationRequest request)
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

        [HttpPost("/reservations/approved")]
        public async Task<ActionResult> ApprovedReservation([FromBody] Reservation reservation)
        {
            var storedReservation = await Context.Reservations.SingleOrDefaultAsync(r => r.Id == reservation.Id);

            if(storedReservation == null)
            {
                return BadRequest("No Pending Reservation with that id");
                // decision time!
                
            } else
            {
                storedReservation.Status = ReservationStatus.Approved;
                // do other stuff, or write a message to the queue that other processes will handle.
                await Context.SaveChangesAsync();
                return Accepted();
            }
            
        }

        [HttpPost("/reservations/cancelled")]
        public async Task<ActionResult> CancelledReservation([FromBody] Reservation reservation)
        {
            var storedReservation = await Context.Reservations.SingleOrDefaultAsync(r => r.Id == reservation.Id);

            if (storedReservation == null)
            {
                return BadRequest("No Pending Reservation with that id");
                // decision time!

            }
            else
            {
                storedReservation.Status = ReservationStatus.Cancelled;
                // do other stuff, or write a message to the queue that other processes will handle.
                await Context.SaveChangesAsync();
                return Accepted();
            }

        }

        [HttpGet("/reservations/approved")]
        public async Task<ActionResult> GetApprovedReservations()
        {
            var response = await Context.Reservations
                .Where(r => r.Status == ReservationStatus.Approved)
                .ToListAsync();
            // TODO: Project these into models. This is not a great way to do it. Classroom only.

            return Ok(response);

        }

        [HttpGet("/reservations/cancelled")]
        public async Task<ActionResult> GetCancelledREservations()
        {
            var response = await Context.Reservations
                .Where(r => r.Status == ReservationStatus.Cancelled)
                .ToListAsync();
            // TODO: Project these into models. This is not a great way to do it. Classroom only.

            return Ok(response);
        }

        [HttpGet("/reservations/pending")]
        public async Task<ActionResult> GetPendingREservations()
        {
            var response = await Context.Reservations
                .Where(r => r.Status == ReservationStatus.Pending)
                .ToListAsync();
            // TODO: Project these into models. This is not a great way to do it. Classroom only.

            return Ok(response);
        }



    }
}
