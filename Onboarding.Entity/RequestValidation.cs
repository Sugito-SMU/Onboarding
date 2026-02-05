using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class RequestValidation
    {
        Dictionary<string, string> _ErrorList = null;

        public RequestValidation()
        {
            _ErrorList = new Dictionary<string, string>();
        }

        public void SetError(string requestField, string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                if (!_ErrorList.ContainsKey(requestField))
                {
                    _ErrorList.Add(requestField, errorMessage);
                }
                else
                {
                    _ErrorList[requestField] += "; " + errorMessage;
                }
            }
        }

        public bool HasError(string requestField)
        {
            if (_ErrorList.ContainsKey(requestField) && !string.IsNullOrWhiteSpace(_ErrorList[requestField]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasError()
        {
            if (_ErrorList.Keys.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetErrorMessage(string requestField)
        {
            if (_ErrorList.ContainsKey(requestField))
            {
                return _ErrorList[requestField];
            }
            else
            {
                return null;
            }
        }

        public List<string> GetAllErrorMessage()
        {
            List<string> errLst = new List<string>();
            foreach (string fieldNm in _ErrorList.Keys)
            {
                if (!string.IsNullOrWhiteSpace(_ErrorList[fieldNm]))
                {
                    errLst.Add(_ErrorList[fieldNm]);
                }
            }
            return errLst;
        }

        public bool HasSectionError(string requestSection)
        {
            foreach (string key in _ErrorList.Keys)
            {
                if (Entity.Constant.RequestSection.Fields[key] == requestSection)
                {
                    return true;
                }
            }
            return false;
        }
        
    }
}
