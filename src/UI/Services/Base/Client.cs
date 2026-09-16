using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Services.Base
{
    public partial class Client : IClient
    {
        // public HttpClient _httpClient { get; }
        public HttpClient HttpClient { get => _httpClient; }
    }
}