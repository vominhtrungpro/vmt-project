using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.models.OpenAI;

namespace vmt_project.services.OpenAI
{
    public interface IChatOpenAIService
    {
        Task<FunctionCallingResponse> FunctionCalling(string text);
    }
}
