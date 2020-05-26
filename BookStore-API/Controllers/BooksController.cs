using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Interacts with the books table
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public BooksController(IBookRepository bookRespository, ILoggerService logger, IMapper mapper)
        {
            _bookRepository = bookRespository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Books
        /// </summary>
        /// <returns>List of Authors</returns> 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo("Accessed book Controller");
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo("Successfully got all books");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{location}:{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Get book by Id
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted call for id:{id}");
                var book = await _bookRepository.FindById(id);
                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"{location}: Successfully got record with id: {id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{location}:{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Creates an author
        /// </summary>
        /// <param name="BookCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO BookCreateDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location} Book Submission Attepmted");
                if (BookCreateDTO == null)
                {
                    _logger.LogWarn($"Empty request was attempted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Book data was incomplete");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(BookCreateDTO);
                var isSuccess = await _bookRepository.Create(book);
                if (!isSuccess)
                {
                    return InternalError($"Book creation failed");
                }

                _logger.LogInfo($"Book created");
                return Created("Create", new { book });

            }
            catch (Exception ex)
            {
                return InternalError($"{location}:{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Updates an author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="BookUpdateDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO BookUpdateDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"Book update Attepmted");
                if (BookUpdateDTO == null || id < 1)
                {
                    _logger.LogWarn($"Empty request was attempted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Author data was incomplete");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(BookUpdateDTO);

                var isSuccess = await _bookRepository.Update(book);
                if (!isSuccess)
                {
                    return InternalError($"Author creation failed");
                }

                _logger.LogInfo($"Author updated");
                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}:{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Deletes a book
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"book delete Attepmted");
                if (id < 1)
                {
                    _logger.LogWarn($"Empty request was attempted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Author data was incomplete");
                    return BadRequest(ModelState);
                }

                var isExist = await _bookRepository.isExist(id);
                if (!isExist)
                {
                    return NotFound();
                }

                var book = await _bookRepository.FindById(id);
                var isSuccess = await _bookRepository.Delete(book);
                if (!isSuccess)
                {
                    return InternalError($"book creation failed");
                }

                _logger.LogInfo($"book updated");
                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}:{ex.Message} - {ex.InnerException}");
            }
        }


        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong");

        }


    }
}
