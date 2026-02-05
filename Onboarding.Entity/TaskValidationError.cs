using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class TaskValidationError
    {
        Dictionary<string, string> _ErrorList = null;

        public TaskValidationError()
        {
            _ErrorList = new Dictionary<string, string>();
        }

        public void SetError(string taskAttrCode, string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {

                if (!_ErrorList.ContainsKey(taskAttrCode))
                {
                    _ErrorList.Add(taskAttrCode, errorMessage);
                }
                else
                {
                    _ErrorList[taskAttrCode] += "; " + errorMessage;
                }
            }
        }

        public bool HasError(string taskAttrCode)
        {
            if (_ErrorList.ContainsKey(taskAttrCode) &&
                !string.IsNullOrWhiteSpace(_ErrorList[taskAttrCode]))
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

        public string GetErrorMessage(string taskAttrCode)
        {
            if (_ErrorList.ContainsKey(taskAttrCode))
            {
                return _ErrorList[taskAttrCode];
            }
            else
            {
                return null;
            }
        }
        
        
    }
}
