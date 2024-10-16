namespace Fliq.Application.Common.Models
{
    public class FaceRectangle
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Target
    {
        public FaceRectangle FaceRectangle { get; set; }
        public string FileName { get; set; }
        public int TimeOffsetWithinFile { get; set; }
        public string ImageType { get; set; }
    }

    public class VerifyResult
    {
        public double MatchConfidence { get; set; }
        public bool IsIdentical { get; set; }
    }

    public class ResponseBody
    {
        public string LivenessDecision { get; set; }
        public Target Target { get; set; }
        public string ModelVersionUsed { get; set; }
        public VerifyResult VerifyResult { get; set; }
    }

    public class Response
    {
        public ResponseBody Body { get; set; }
        public int StatusCode { get; set; }
        public int LatencyInMilliseconds { get; set; }
    }

    public class Request
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string UserAgent { get; set; }
    }

    public class LivelinessResult
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string RequestId { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
        public string Digest { get; set; }
    }

    public class LivenessSessionResult
    {
        public string Status { get; set; }
        public LivelinessResult Result { get; set; }
        public string Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int AuthTokenTimeToLiveInSeconds { get; set; }
        public string DeviceCorrelationId { get; set; }
        public bool SessionExpired { get; set; }
    }
}