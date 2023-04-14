using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTR.HWExt.Peripherals
{
    public class ResponseMessage
    {
        protected NETS netsPayment;
        //private ResponseModel response;

        public ResponseMessage(NETS _NETSPayment)
        {
            netsPayment = _NETSPayment;
        }

        public virtual ResponseModel ConvertValues(string _indata)
        {
            // Conversion Part 
            string ConvertedResponse = ConvertToHEX(_indata);
            //separate the HEX value stream into designated field.
            string[] ResponseValues = SeparateAllResponseFields(ConvertedResponse);
            //Convert Hex Values Array to readable string array
            string[] ReadableStrings = ConvertToString(ResponseValues);

            return this.SetResponseTransactionFields(ReadableStrings);
        }
        
        //set separate for all fields
        protected string[] SeparateAllResponseFields(string ResponseString)
        {
            string[] Values;
            string[] Separator = { "1C" };
            Values = ResponseString.Split(Separator, StringSplitOptions.None);

            return Values;
        }        

        //convert string to hex
        protected string ConvertToHEX(string RawResponseString)
        {
            //List<string> HexVals = new List<string>();
            string ConvertedValue = string.Empty;
            foreach (char letter in RawResponseString)
            {
                // Get the integral value of the character. 
                int value = Convert.ToInt32(letter);

                // Convert the decimal value to a hexadecimal value in string form. 
                string hexOutput = String.Format("{0:X}", value);

                ConvertedValue += hexOutput + " ";
            }
            return ConvertedValue;
        }

        //Convert HEX Values into String
        protected string[] ConvertToString(string[] HEXValueArrays)
        {
            List<string> ret = new List<string>();

            foreach (string Field in HEXValueArrays)
            {
                Field.Trim();
                string[] stringSeparators = new string[] { " " };
                string[] result;
                string convertedString = string.Empty;
                char charValue = new char();
                result = Field.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                foreach (string FieldChar in result)
                {
                    int value = Convert.ToInt32(FieldChar, 16);
                    charValue = (char)value;
                    convertedString += charValue.ToString();
                }
                ret.Add(convertedString);
            }
            return ret.ToArray();
        }

        //set response model data
        protected virtual ResponseModel SetResponseTransactionFields(string[] ResponseString)
        {
            //this class needs to be overridden
            throw new System.ArgumentNullException("SetResponseTransactionFields method needs to be overridden");            
        }

    }
}
