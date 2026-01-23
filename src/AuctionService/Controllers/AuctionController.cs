using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionService.Data;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AuctionService.DTOs;
using Microsoft.EntityFrameworkCore; // Add this if AuctionDto is in DTOs namespace
using AuctionService.Entities; // Add this line if Auction is in Models namespace

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await _context.Auctions.Include(x => x.Item)
            .OrderBy(x => x.Item.Make).ToListAsync();
            return _mapper.Map<List<AuctionDto>>(auctions);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            else
            {
                return _mapper.Map<AuctionDto>(auction);
            }
        }
        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = _mapper.Map<Auction>(createAuctionDto);
            _context.Auctions.Add(auction);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest("Failed to create auction");
            }
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, auctionDto);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Year = updateAuctionDto.Year != 0 ? updateAuctionDto.Year : auction.Item.Year;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.ImageUrl = updateAuctionDto.ImageUrl ?? auction.Item.ImageUrl;
            auction.ReservePrice = updateAuctionDto.ReservePrice != 0 ? updateAuctionDto.ReservePrice : auction.ReservePrice;
            auction.AuctionEnd = updateAuctionDto.AuctionEnd != default(DateTime) ? updateAuctionDto.AuctionEnd : auction.AuctionEnd;
            auction.UpdatedAt = DateTime.UtcNow;
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest("Failed to update auction");
            }

            return Ok(_mapper.Map<AuctionDto>(auction));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            _context.Auctions.Remove(auction);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest("Failed to delete auction");
            }
            return Ok();
        }
    }
}
