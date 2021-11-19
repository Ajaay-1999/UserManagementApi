using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagementApi.DataAccess;
using UserManagementApi.Model;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserManagementApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IDbServices dbServices;
        public UsersController(IDbServices dbServices)
        {
            this.dbServices = dbServices;
        }

        /// <summary>
        /// To Get the List of User details from database
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="fetch"></param>
        /// <returns></returns>
        /// <response code="200">Request has been executed Successfully</response>
        /// <response code="404">Encounted Error while processing request</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<User>>> GetUserDetails(int offset = 0, int fetch = 10)
        {
            var userList = await dbServices.GetUserList(offset, fetch);
            if (userList == null)
                return NotFound();
            return Ok(userList);
        }
        /// <summary>
        /// To Get a specific User by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">Request has been executed Successfully</response>
        /// <response code="404">Encounted Error while processing request</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUserById(int userId)
        {
            var user =  await dbServices.GetUser(userId);
            if (user == null)
                return NotFound("User Id Not Found");
            return user;
        }
        /// <summary>
        /// Create a User and adds the user in the database Table
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <response code="200">Request has been executed Successfully</response>
        /// <response code="404">Encounted Error while processing request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> CreateUser([FromBody] User value)
        {
            var result = await dbServices.CreateUser(value);
            if (!result)
                return NotFound();
            return Ok("User Created Successfully");
        }
        /// <summary>
        /// Search the texts in database by specific keyword
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        /// <response code="200">Request has been executed Successfully</response>
        /// <response code="404">Encounted Error while processing request</response>
        [HttpPost, Route("Search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<User>>> SearchUser(string Search)
        {
            var userList = await dbServices.SearchUser(Search);
            if (userList == null)
                return NotFound();
            return Ok(userList);
        }
        /// <summary>
        /// Delete a user by its Id in the Database
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">Request has been executed Successfully</response>
        /// <response code="404">Encounted Error while processing request</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeleteUser(int userId)
        {
            var result = await dbServices.DeleteUser(userId);
            if (!result)
                return NotFound();
            return Ok("Record Deleted Successfully!");
        }
        /// <summary>
        /// Updates a user details for a specific record in the database
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        /// <response code="200">Request has been executed Successfully</response>
        /// <response code="404">Encounted Error while processing request</response>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> UpdateUser(User updateUser)
        {
            var result = await dbServices.UpdateUserDetails(updateUser);
            if (!result)
                return NotFound();
            return Ok("User Details Updated Successfully");
        }
    }
}
