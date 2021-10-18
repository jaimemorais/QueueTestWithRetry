using Microsoft.AspNetCore.Mvc;

namespace QueueWithRetry
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly QueueWithRetryProcessor _queueWithRetryProcessor;

        public ValuesController(QueueWithRetryProcessor queueWithRetryProcessor)
        {
            _queueWithRetryProcessor = queueWithRetryProcessor;
        }



        [HttpPost]
        public ActionResult Post()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                string msg = reader.ReadToEndAsync().GetAwaiter().GetResult();

                _queueWithRetryProcessor.RegisterDocument(msg);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.Message);

                return StatusCode(500);
            }
            
        }

    }
}
