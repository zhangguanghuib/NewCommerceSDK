using PCEFTPOS.EFTClient.IPInterface.Slave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PCEFTPOS.EFTClient.IPInterface
{
	public interface IMessageParser
	{
		EFTResponse StringToEFTResponse(string msg);
		EFTResponse XMLStringToEFTResponse(string msg);
		string EFTRequestToString(EFTRequest eftRequest);
		string EFTRequestToXMLString(EFTRequest eftRequest);
	}

	public class DefaultMessageParser : IMessageParser
	{
		ReceiptType lastReceiptType;

		enum IPClientResponseType
		{
			Logon = 'G', Transaction = 'M', QueryCard = 'J', Configure = '1', ControlPanel = '5', SetDialog = '2', Settlement = 'P',
			DuplicateReceipt = 'C', GetLastTransaction = 'N', Status = 'K', Receipt = '3', Display = 'S', GenericPOSCommand = 'X', PINRequest = 'W',
			ChequeAuth = 'H', SendKey = 'Y', ClientList = 'Q', CloudLogon = 'A'
		}

		#region StringToEFTResponse
		/// <summary> Parses a string to an EFTResponse message </summary>
		/// <param name="msg">string to parse</param>
		/// <returns>An EFTResponse message</returns>
		/// <exception cref="ArgumentException">An ArgumentException is thrown if the contents of msg is invalid</exception>
		public EFTResponse StringToEFTResponse(string msg)
		{
			if (msg?.Length < 1)
			{
				throw new ArgumentException("msg is null or zero length", nameof(msg));
			}

			EFTResponse eftResponse = null;
			switch ((IPClientResponseType)msg[0])
			{
				case IPClientResponseType.Display:
					eftResponse = ParseDisplayResponse(msg);
					break;
				case IPClientResponseType.Receipt:
					eftResponse = ParseReceiptResponse(msg);
					break;
				case IPClientResponseType.Logon:
					eftResponse = ParseEFTLogonResponse(msg);
					break;
				case IPClientResponseType.Transaction:
					eftResponse = ParseEFTTransactionResponse(msg);
					break;
				case IPClientResponseType.SetDialog:
					eftResponse = ParseSetDialogResponse(msg);
					break;
				case IPClientResponseType.GetLastTransaction:
					eftResponse = ParseEFTGetLastTransactionResponse(msg);
					break;
				case IPClientResponseType.DuplicateReceipt:
					eftResponse = ParseEFTReprintReceiptResponse(msg);
					break;
				case IPClientResponseType.ControlPanel:
					eftResponse = ParseControlPanelResponse(msg);
					break;
				case IPClientResponseType.Settlement:
					eftResponse = ParseEFTSettlementResponse(msg);
					break;
				case IPClientResponseType.Status:
					eftResponse = ParseEFTStatusResponse(msg);
					break;
				case IPClientResponseType.ChequeAuth:
					eftResponse = ParseChequeAuthResponse(msg);
					break;
				case IPClientResponseType.QueryCard:
					eftResponse = ParseQueryCardResponse(msg);
					break;
				case IPClientResponseType.GenericPOSCommand:
					eftResponse = ParseGenericPOSCommandResponse(msg);
					break;
				case IPClientResponseType.Configure:
					eftResponse = ParseConfigMerchantResponse(msg);
					break;
				case IPClientResponseType.CloudLogon:
					eftResponse = ParseCloudLogonResponse(msg);
					break;
				case IPClientResponseType.ClientList:
					eftResponse = ParseClientListResponse(msg);
					break;

				default:
					throw new ArgumentException($"Unknown message type: {msg}", nameof(msg));
			}

			return eftResponse;
		}


		T TryParse<T>(string input, int length, ref int index)
		{
			return TryParse<T>(input, length, ref index, "");
		}

		T TryParse<T>(string input, int length, ref int index, string format)
		{
			T result = default(T);

			if (input.Length - index >= length)
			{
				if (result is bool && length == 1)
				{
					result = (T)Convert.ChangeType((input[index] == '1' || input[index] == 'Y'), typeof(T));
					index += length;
				}
				else
				{
					object data = input.Substring(index, length);
					try
					{
						if (result is Enum && length == 1)
							result = (T)Enum.ToObject(typeof(T), ((string)data)[0]);
						else if (result is DateTime && format.Length > 1)
							result = (T)(object)DateTime.ParseExact((string)data, format, null);
						else
							result = (T)Convert.ChangeType(data, typeof(T));
					}
					catch
					{
						var idx = index;
						//Log(LogLevel.Error, tr => tr.Set($"Unable to parse field. Input={input}, Index={idx}, Length={length}"));
					}
					finally
					{
						index += length;
					}
				}
			}
			else
				index = length;

			return result;
		}

		EFTResponse ParseEFTTransactionResponse(string msg)
		{
			var index = 1;

			var r = new EFTTransactionResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);
			r.Merchant = TryParse<string>(msg, 2, ref index);
			r.TxnType = TryParse<TransactionType>(msg, 1, ref index);
			r.CardAccountType = r.CardAccountType.FromString(TryParse<string>(msg, 7, ref index));
			r.AmtCash = TryParse<decimal>(msg, 9, ref index) / 100;
			r.AmtPurchase = TryParse<decimal>(msg, 9, ref index) / 100;
			r.AmtTip = TryParse<decimal>(msg, 9, ref index) / 100;
			r.AuthCode = TryParse<int>(msg, 6, ref index);
			r.TxnRef = TryParse<string>(msg, 16, ref index);
			r.Stan = TryParse<int>(msg, 6, ref index);
			r.Caid = TryParse<string>(msg, 15, ref index);
			r.Catid = TryParse<string>(msg, 8, ref index);
			r.DateExpiry = TryParse<string>(msg, 4, ref index);
			r.SettlementDate = TryParse<DateTime>(msg, 4, ref index, "ddMM");
			r.Date = TryParse<DateTime>(msg, 12, ref index, "ddMMyyHHmmss");
			r.CardType = TryParse<string>(msg, 20, ref index);
			r.Pan = TryParse<string>(msg, 20, ref index);
			r.Track2 = TryParse<string>(msg, 40, ref index);
			r.RRN = TryParse<string>(msg, 12, ref index);
			r.CardName = TryParse<int>(msg, 2, ref index);
			r.TxnFlags = new TxnFlags(TryParse<string>(msg, 8, ref index).ToCharArray());
			r.BalanceReceived = TryParse<bool>(msg, 1, ref index);
			r.AvailableBalance = TryParse<decimal>(msg, 9, ref index) / 100;
			r.ClearedFundsBalance = TryParse<decimal>(msg, 9, ref index) / 100;
			r.PurchaseAnalysisData = new PadField(TryParse<string>(msg, msg.Length - index, ref index));

			return r;
		}

		EFTResponse ParseEFTGetLastTransactionResponse(string msg)
		{
			var index = 1;

			var r = new EFTGetLastTransactionResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.LastTransactionSuccess = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);
			r.Merchant = TryParse<string>(msg, 2, ref index);

			if (char.IsLower(msg[index]))
			{
				r.IsTrainingMode = true;
				msg = msg.Substring(0, index) + char.ToUpper(msg[index]) + msg.Substring(index + 1);
			}
			r.TxnType = TryParse<TransactionType>(msg, 1, ref index);
			string accountType = TryParse<string>(msg, 7, ref index);
			if (accountType == "Credit ") r.CardAccountType = AccountType.Credit;
			else if (accountType == "Savings") r.CardAccountType = AccountType.Savings;
			else if (accountType == "Cheque ") r.CardAccountType = AccountType.Cheque;
			else r.CardAccountType = AccountType.Default;
			r.AmtCash = TryParse<decimal>(msg, 9, ref index) / 100;
			r.AmtPurchase = TryParse<decimal>(msg, 9, ref index) / 100;
			r.AmtTip = TryParse<decimal>(msg, 9, ref index) / 100;
			r.AuthCode = TryParse<int>(msg, 6, ref index);
			r.TxnRef = TryParse<string>(msg, 16, ref index);
			r.Stan = TryParse<int>(msg, 6, ref index);
			r.Caid = TryParse<string>(msg, 15, ref index);
			r.Catid = TryParse<string>(msg, 8, ref index);
			r.DateExpiry = TryParse<string>(msg, 4, ref index);
			r.SettlementDate = TryParse<DateTime>(msg, 4, ref index, "ddMM");
			r.BankDateTime = TryParse<DateTime>(msg, 12, ref index, "ddMMyyHHmmss");
			r.CardType = TryParse<string>(msg, 20, ref index);
			r.Pan = TryParse<string>(msg, 20, ref index);
			r.Track2 = TryParse<string>(msg, 40, ref index);
			r.RRN = TryParse<string>(msg, 12, ref index);
			r.CardName = TryParse<int>(msg, 2, ref index);
			string txnFlags = TryParse<string>(msg, 8, ref index);
			r.TxnFlags = new TxnFlags(txnFlags.ToCharArray());
			r.BalanceReceived = TryParse<bool>(msg, 1, ref index);
			r.AvailableBalance = TryParse<decimal>(msg, 9, ref index) / 100;
			int padLength = TryParse<int>(msg, 3, ref index);
			r.PurchaseAnalysisData = new PadField(TryParse<string>(msg, padLength, ref index));

			return r;
		}

		EFTResponse ParseSetDialogResponse(string msg)
		{
			var index = 1;

			var r = new SetDialogResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);

			return r;
		}

		EFTResponse ParseEFTLogonResponse(string msg)
		{
			var index = 1;

			var r = new EFTLogonResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);

			if (msg.Length > 25)
			{
				r.Catid = TryParse<string>(msg, 8, ref index);
				r.Caid = TryParse<string>(msg, 15, ref index);
				r.Date = TryParse<DateTime>(msg, 12, ref index, "ddMMyyHHmmss");
				r.Stan = TryParse<int>(msg, 6, ref index);
				r.PinPadVersion = TryParse<string>(msg, 16, ref index);
				r.PurchaseAnalysisData = new PadField(TryParse<string>(msg, msg.Length - index, ref index));
			}
			return r;
		}

		EFTResponse ParseDisplayResponse(string msg)
		{
			int index = 1;

			var r = new EFTDisplayResponse();
			index++; // Skip sub code.
			r.NumberOfLines = TryParse<int>(msg, 2, ref index);
			r.LineLength = TryParse<int>(msg, 2, ref index);
			for (int i = 0; i < r.NumberOfLines; i++)
				r.DisplayText[i] = TryParse<string>(msg, r.LineLength, ref index);
			r.CancelKeyFlag = TryParse<bool>(msg, 1, ref index);
			r.AcceptYesKeyFlag = TryParse<bool>(msg, 1, ref index);
			r.DeclineNoKeyFlag = TryParse<bool>(msg, 1, ref index);
			r.AuthoriseKeyFlag = TryParse<bool>(msg, 1, ref index);
			r.InputType = TryParse<InputType>(msg, 1, ref index);
			r.OKKeyFlag = TryParse<bool>(msg, 1, ref index);
			index += 2;
			r.GraphicCode = TryParse<GraphicCode>(msg, 1, ref index);
			int padLength = TryParse<int>(msg, 3, ref index);
			r.PurchaseAnalysisData = new PadField(TryParse<string>(msg, padLength, ref index));

			return r;
		}

		EFTResponse ParseReceiptResponse(string msg)
		{
			int index = 1;

			var r = new EFTReceiptResponse
			{
				Type = TryParse<ReceiptType>(msg, 1, ref index)
			};

			if (r.Type != ReceiptType.ReceiptText)
			{
				lastReceiptType = r.Type;
				r.IsPrePrint = true;
			}
			else
			{
				List<string> receiptLines = new List<string>();
				bool done = false;
				while (!done)
				{
					int lineLength = msg.Substring(index).IndexOf("\r\n");
					if (lineLength > 0)
					{
						receiptLines.Add(msg.Substring(index, lineLength));
						index += lineLength + 2;
						if (index >= msg.Length)
							done = true;
					}
					else
						done = true;
				}

				r.ReceiptText = receiptLines.ToArray();
				r.Type = lastReceiptType;
			}

			return r;
		}

		EFTResponse ParseControlPanelResponse(string msg)
		{
			int index = 1;

			var r = new EFTControlPanelResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);

			return r;
		}

		EFTResponse ParseEFTReprintReceiptResponse(string msg)
		{
			int index = 1;

			var r = new EFTReprintReceiptResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);
			// index will be 20 if the TryParse above fails i.e. if the msg is shorter than index + 20. So index is 20 if there is no more message. If there's no more message then we return what we have and don't try to parse a receipt.
			if (index == 20)
			{
				return r;
			}
			// If we get here then there is more message and so we should try to parse a receipt.
			List<string> receiptLines = new List<string>();
			bool done = false;
			while (!done)
			{
				int lineLength = msg.Substring(index).IndexOf("\r\n");
				if (lineLength > 0)
				{
					receiptLines.Add(msg.Substring(index, lineLength));
					index += lineLength + 2;
					if (index >= msg.Length)
						done = true;
				}
				else
					done = true;
			}

			r.ReceiptText = receiptLines.ToArray();

			return r;
		}

		EFTResponse ParseEFTSettlementResponse(string msg)
		{
			var index = 1;

			var r = new EFTSettlementResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);

			if (msg.Length > 25)
			{
				r.SettlementData = msg.Substring(index);

				//int cardCount = int.Parse( response.Substring( index, 9 ) ); index += 9;
				//for( int i = 0; i < cardCount; i++ )
				//{
				//    int cardTotalsDataLength = int.Parse( response.Substring( index, 3 ) ); index += 3;
				//    if( cardTotalsDataLength >= 69 )
				//    {
				//        SettlementCardTotals cardTotals = new SettlementCardTotals();
				//        cardTotals.CardName = response.Substring( index, 20 ); index += 20;
				//        try { cardTotals.PurchaseAmount = decimal.Parse( response.Substring( index, 9 ) ) / 100; }
				//        catch { cardTotals.PurchaseAmount = 0; }
				//        finally { index += 9; }
				//        try { cardTotals.PurchaseCount = int.Parse( response.Substring( index, 3 ) ); }
				//        catch { cardTotals.PurchaseCount = 0; }
				//        finally { index += 3; }
				//        try { cardTotals.CashOutAmount = decimal.Parse( response.Substring( index, 9 ) ) / 100; }
				//        catch { cardTotals.CashOutAmount = 0; }
				//        finally { index += 9; }
				//        try { cardTotals.CashOutCount = int.Parse( response.Substring( index, 3 ) ); }
				//        catch { cardTotals.CashOutCount = 0; }
				//        finally { index += 3; }
				//        try { cardTotals.RefundAmount = decimal.Parse( response.Substring( index, 9 ) ) / 100; }
				//        catch { cardTotals.RefundAmount = 0; }
				//        finally { index += 9; }
				//        try { cardTotals.RefundCount = int.Parse( response.Substring( index, 3 ) ); }
				//        catch { cardTotals.RefundCount = 0; }
				//        finally { index += 3; }
				//        try { cardTotals.TotalAmount = decimal.Parse( response.Substring( index, 10 ), System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowLeadingWhite ) / 100; }
				//        catch { cardTotals.TotalAmount = 0; }
				//        finally { index += 9; }
				//        try { cardTotals.TotalCount = int.Parse( response.Substring( index, 3 ) ); }
				//        catch { cardTotals.TotalCount = 0; }
				//        finally { index += 3; }
				//        displayResponse.SettlementCardData.Add( cardTotals );
				//        index += cardTotalsDataLength - 69;
				//    }
				//}

				//int totalsDataLength = int.Parse( response.Substring( index, 3 ) ); index += 3;
				//if( totalsDataLength >= 69 )
				//{
				//    SettlementTotals settleTotals = new SettlementTotals();
				//    settleTotals.TotalsDescription = response.Substring( index, 20 ); index += 20;
				//    try { settleTotals.PurchaseAmount = decimal.Parse( response.Substring( index, 9 ) ) / 100; }
				//    catch { settleTotals.PurchaseAmount = 0; }
				//    finally { index += 9; }
				//    try { settleTotals.PurchaseCount = int.Parse( response.Substring( index, 3 ) ); }
				//    catch { settleTotals.PurchaseCount = 0; }
				//    finally { index += 3; }
				//    try { settleTotals.CashOutAmount = decimal.Parse( response.Substring( index, 9 ) ) / 100; }
				//    catch { settleTotals.CashOutAmount = 0; }
				//    finally { index += 9; }
				//    try { settleTotals.CashOutCount = int.Parse( response.Substring( index, 3 ) ); }
				//    catch { settleTotals.CashOutCount = 0; }
				//    finally { index += 3; }
				//    try { settleTotals.RefundAmount = decimal.Parse( response.Substring( index, 9 ) ) / 100; }
				//    catch { settleTotals.RefundAmount = 0; }
				//    finally { index += 9; }
				//    try { settleTotals.RefundCount = int.Parse( response.Substring( index, 3 ) ); }
				//    catch { settleTotals.RefundCount = 0; }
				//    finally { index += 3; }
				//    try { settleTotals.TotalAmount = decimal.Parse( response.Substring( index, 10 ), System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowLeadingWhite ) / 100; }
				//    catch { settleTotals.TotalAmount = 0; }
				//    finally { index += 9; }
				//    try { settleTotals.TotalCount = int.Parse( response.Substring( index, 3 ) ); }
				//    catch { settleTotals.TotalCount = 0; }
				//    finally { index += 3; }
				//    displayResponse.TotalsData = settleTotals;
				//    index += totalsDataLength - 69;
				//}

				//int padLength;
				//try
				//{
				//    padLength = int.Parse( response.Substring( index, 3 ) );
				//    displayResponse.PurchaseAnalysisData = response.Substring( index, padLength ); index += padLength;
				//}
				//catch { padLength = 0; }
				//finally { index += 3; }
			}

			return r;
		}

		EFTResponse ParseQueryCardResponse(string msg)
		{
			int index = 1;

			var r = new EFTQueryCardResponse
			{
				AccountType = (AccountType)msg[index++],
				Success = TryParse<bool>(msg, 1, ref index),
				ResponseCode = TryParse<string>(msg, 2, ref index),
				ResponseText = TryParse<string>(msg, 20, ref index)
			};

			if (msg.Length > 25)
			{
				r.Track2 = msg.Substring(index, 40); index += 40;
				string track1or3 = msg.Substring(index, 80); index += 80;
				char trackFlag = msg[index++];
				switch (trackFlag)
				{
					case '1':
						r.TrackFlags = TrackFlags.Track1;
						r.Track1 = track1or3;
						break;
					case '2':
						r.TrackFlags = TrackFlags.Track2;
						break;
					case '3':
						r.TrackFlags = TrackFlags.Track1 | TrackFlags.Track2;
						r.Track1 = track1or3;
						break;
					case '4':
						r.TrackFlags = TrackFlags.Track3;
						r.Track3 = track1or3;
						break;
					case '6':
						r.TrackFlags = TrackFlags.Track2 | TrackFlags.Track3;
						r.Track3 = track1or3;
						break;
				}

				r.CardName = int.Parse(msg.Substring(index, 2)); index += 2;

				int padLength;
				try
				{
					padLength = int.Parse(msg.Substring(index, 3));
					r.PurchaseAnalysisData = new PadField(msg.Substring(index, padLength));
					index += padLength;
				}
				catch { padLength = 0; }
				finally { index += 3; }
			}

			return r;
		}

		EFTResponse ParseClientListResponse(string msg)
		{
			var r = new EFTClientListResponse();
			// Get rid of the junk at the beginning, will look like #Q000001
			string trimmedMsg = msg.Substring(msg.IndexOf('{') + 1);
			// Each client coming in will be in format {CLIENTNAME,IPADDRESS,PORT,STATUS}|{CLIENTNAME,IPADDRESS,PORT,STATUS} So we split on the | character
			if (trimmedMsg.IndexOf('|') > -1)
			{
				string[] clients = trimmedMsg.Split('|');
				foreach (string client in clients)
				{
					EFTClientListResponse.EFTClient newClient = new EFTClientListResponse.EFTClient();
					// Each client 'string' will be surrounded by {} so we get rid of them
					string trimmedClient = client.TrimEnd('}').TrimStart('{');
					// That leaves us with the CLIENTNAME,IPADDRESS,PORT,STATUS and we split on ',' to get individual properties
					string[] newClientProps = trimmedClient.Split(',');
					newClient.Name = newClientProps[0];
					newClient.IPAddress = newClientProps[1];
					newClient.Port = Convert.ToInt32(newClientProps[2]);
					// EFTClientListResponse.Client.State is an enum, so we need to check the string and parse into the enum
					if (newClientProps[3].Equals("AVAILABLE"))
					{
						newClient.State = EFTClientListResponse.EFTClientState.Available;
					}
					else
						newClient.State = EFTClientListResponse.EFTClientState.Unavailable;
					// Add the new client to the response list
					r.EFTClients.Add(newClient);
				}
			}
			else if (msg.IndexOf('|') == -1)
			{
				EFTClientListResponse.EFTClient newClient = new EFTClientListResponse.EFTClient();
				// Each client 'string' will be surrounded by {} so we get rid of them
				string trimmedClient = trimmedMsg.TrimEnd('}').TrimStart('{');
				// That leaves us with the CLIENTNAME,IPADDRESS,PORT,STATUS and we split on ',' to get individual properties
				string[] newClientProps = trimmedMsg.Split(',');
				newClient.Name = newClientProps[0];
				newClient.IPAddress = newClientProps[1];
				newClient.Port = Convert.ToInt32(newClientProps[2]);
				// EFTClientListResponse.Client.State is an enum, so we need to check the string and parse into the enum
				if (newClientProps[3].Equals("AVAILABLE"))
				{
					newClient.State = EFTClientListResponse.EFTClientState.Available;
				}
				else
					newClient.State = EFTClientListResponse.EFTClientState.Unavailable;
				// Add the new client to the response list
				r.EFTClients.Add(newClient);
			}
			return r;
		}

		EFTResponse ParseEFTConfigureMerchantResponse(string msg)
		{
			int index = 1;

			var r = new EFTConfigureMerchantResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);

			return r;
		}

		EFTResponse ParseEFTStatusResponse(string msg)
		{
			int index = 1;

			var r = new EFTStatusResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);
			if (index >= msg.Length) return r;
			r.Merchant = TryParse<string>(msg, 2, ref index);
			r.AIIC = TryParse<string>(msg, 11, ref index);
			r.NII = TryParse<int>(msg, 3, ref index);
			r.Caid = TryParse<string>(msg, 15, ref index);
			r.Catid = TryParse<string>(msg, 8, ref index);
			r.Timeout = TryParse<int>(msg, 3, ref index);
			r.LoggedOn = TryParse<bool>(msg, 1, ref index);
			r.PinPadSerialNumber = TryParse<string>(msg, 16, ref index);
			r.PinPadVersion = TryParse<string>(msg, 16, ref index);
			r.BankDescription = TryParse<string>(msg, 32, ref index);
			int padLength = TryParse<int>(msg, 3, ref index);
			if (msg.Length - index < padLength)
				return r;
			r.SAFCount = TryParse<int>(msg, 4, ref index);
			r.NetworkType = TryParse<NetworkType>(msg, 1, ref index);
			r.HardwareSerial = TryParse<string>(msg, 16, ref index);
			r.RetailerName = TryParse<string>(msg, 40, ref index);
			r.OptionsFlags = ParseStatusOptionFlags(msg.Substring(index, 32).ToCharArray()); index += 32;
			r.SAFCreditLimit = TryParse<int>(msg, 9, ref index) / 100;
			r.SAFDebitLimit = TryParse<int>(msg, 9, ref index) / 100;
			r.MaxSAF = TryParse<int>(msg, 3, ref index);
			r.KeyHandlingScheme = ParseKeyHandlingType(msg[index++]);
			r.CashoutLimit = TryParse<decimal>(msg, 9, ref index) / 100;
			r.RefundLimit = TryParse<decimal>(msg, 9, ref index) / 100;
			r.CPATVersion = TryParse<string>(msg, 6, ref index);
			r.NameTableVersion = TryParse<string>(msg, 6, ref index);
			r.TerminalCommsType = ParseTerminalCommsType(msg[index++]);
			r.CardMisreadCount = TryParse<int>(msg, 6, ref index);
			r.TotalMemoryInTerminal = TryParse<int>(msg, 4, ref index);
			r.FreeMemoryInTerminal = TryParse<int>(msg, 4, ref index);
			r.EFTTerminalType = ParseEFTTerminalType(msg.Substring(index, 4)); index += 4;
			r.NumAppsInTerminal = TryParse<int>(msg, 2, ref index);
			r.NumLinesOnDisplay = TryParse<int>(msg, 2, ref index);
			r.HardwareInceptionDate = TryParse<DateTime>(msg, 6, ref index, "ddMMyy");

			return r;
		}

		TerminalCommsType ParseTerminalCommsType(char CommsType)
		{
			TerminalCommsType commsType = TerminalCommsType.Unknown;

			if (CommsType == '0') commsType = TerminalCommsType.Cable;
			else if (CommsType == '1') commsType = TerminalCommsType.Infrared;

			return commsType;
		}

		KeyHandlingType ParseKeyHandlingType(char KeyHandlingScheme)
		{
			KeyHandlingType keyHandlingType = KeyHandlingType.Unknown;

			if (KeyHandlingScheme == '0') keyHandlingType = KeyHandlingType.SingleDES;
			else if (KeyHandlingScheme == '1') keyHandlingType = KeyHandlingType.TripleDES;

			return keyHandlingType;
		}

		EFTTerminalType ParseEFTTerminalType(string TerminalType)
		{
			EFTTerminalType terminalType = EFTTerminalType.Unknown;

			if (TerminalType == "0062") terminalType = EFTTerminalType.IngenicoNPT710;
			else if (TerminalType == "0069") terminalType = EFTTerminalType.IngenicoPX328;
			else if (TerminalType == "7010") terminalType = EFTTerminalType.Ingenicoi3070;
			else if (TerminalType == "5110") terminalType = EFTTerminalType.Ingenicoi5110;
			else if (TerminalType == "0006") terminalType = EFTTerminalType.IngenicoIxx250;
			else if (TerminalType == "i050") terminalType = EFTTerminalType.IngenicoMove5000;
			else if (TerminalType == "p010") terminalType = EFTTerminalType.PCEFTPOSVirtualPinpad;
			else if (TerminalType.ToLower() == "albt") terminalType = EFTTerminalType.Albert;
			else if (TerminalType == "X690") terminalType = EFTTerminalType.VerifoneVx690;
			else if (TerminalType == "0820") terminalType = EFTTerminalType.VerifoneVx820;

			return terminalType;
		}

		PINPadOptionFlags ParseStatusOptionFlags(char[] Flags)
		{
			PINPadOptionFlags flags = 0;
			int index = 0;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.Tipping;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.PreAuth;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.Completions;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.CashOut;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.Refund;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.Balance;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.Deposit;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.Voucher;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.MOTO;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.AutoCompletion;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.EFB;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.EMV;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.Training;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.Withdrawal;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.Transfer;
			if (Flags[index++] == '1') flags |= PINPadOptionFlags.StartCash;
			return flags;
		}

		EFTResponse ParseChequeAuthResponse(string msg)
		{
			int index = 1;

			var r = new EFTChequeAuthResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);

			if (msg.Length > 25)
			{
				r.Merchant = TryParse<string>(msg, 2, ref index);
				try { r.Amount = decimal.Parse(msg.Substring(index, 9)) / 100; }
				catch { r.Amount = 0; }
				finally { index += 9; }
				try { r.AuthNumber = int.Parse(msg.Substring(index, 6)); }
				catch { r.AuthNumber = 0; }
				finally { index += 6; }
				r.ReferenceNumber = msg.Substring(index, 12); index += 12;
			}

			return r;
		}

		EFTResponse ParseGenericPOSCommandResponse(string msg)
		{
			// Validate response length
			if (string.IsNullOrEmpty(msg))
			{
				return null;
			}

			int index = 1;

			CommandType commandType = (CommandType)msg[index++];
			switch (commandType)
			{
				case CommandType.GetPassword:
					var pwdResponse = new EFTGetPasswordResponse
					{
						ResponseCode = msg.Substring(index, 2)
					};
					index += 2;
					pwdResponse.Success = pwdResponse.ResponseCode == "00";
					pwdResponse.ResponseText = msg.Substring(index, 20); index += 20;

					if (msg.Length > 25)
					{
						int pwdLength = 0;
						try { pwdLength = int.Parse(msg.Substring(index, 2)); }
						finally { index += 2; }
						pwdResponse.Password = msg.Substring(index, pwdLength); index += pwdLength;
					}
					return pwdResponse;
				case CommandType.Slave:
					var slaveResponse = new EFTSlaveResponse
					{
						ResponseCode = msg.Substring(index, 2)
					};
					index += 2;
					slaveResponse.Response = msg.Substring(index);
					return slaveResponse;

				case CommandType.PayAtTable:
					var patResponse = new EFTPayAtTableResponse();
					index = 22;

					var headerLength = msg.Substring(index, 6); index += 6;
					int len = 0;
					int.TryParse(headerLength, out len);

					patResponse.Header = msg.Substring(index, len); index += len;
					patResponse.Content = msg.Substring(index, msg.Length - index);

					return patResponse;

				case CommandType.BasketData:
					return ParseBasketDataResponse(msg);
			}

			return null;
		}

		EFTResponse ParseConfigMerchantResponse(string msg)
		{
			int index = 1;

			var r = new EFTConfigureMerchantResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);

			return r;
		}

		EFTResponse ParseCloudLogonResponse(string msg)
		{
			int index = 1;

			var r = new EFTCloudLogonResponse();
			index++; // Skip sub code.
			r.Success = TryParse<bool>(msg, 1, ref index);
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);

			return r;
		}

		EFTResponse ParseBasketDataResponse(string msg)
		{
			int index = 1; // msg[0] is the command code

			var r = new EFTBasketDataResponse();
			index++; // Skip sub code.
			r.ResponseCode = TryParse<string>(msg, 2, ref index);
			r.ResponseText = TryParse<string>(msg, 20, ref index);

			r.Success = r.ResponseCode == "00";

			return r;
		}


		/// <summary>
		/// Convert a PC-EFTPOS message (e.g. #0010K0000) to a human readable debug string
		/// </summary>
		public static string MsgToDebugString(string msg)
		{
			if ((msg?.Length ?? 0) < 2)
			{
				return $"Unable to parse msg{Environment.NewLine}ContentLength={msg?.Length ?? 0}{Environment.NewLine}Content={msg}";
			}

			// Remove the header if one exists
			if (msg[0] == '#')
			{
				if (msg.Length < 7)
					return $"Unable to parse msg{Environment.NewLine}ContentLength={msg?.Length ?? 0}{Environment.NewLine}Content={msg}";

				msg = msg.Substring(5);
			}

			var messageParser = new DefaultMessageParser();
			try
			{
				var eftResponse = messageParser.StringToEFTResponse(msg);
				return PrintProperties(eftResponse);
			}
			catch (ArgumentException)
			{
				// Try StringToEFTRequest()
				return $"Unable to parse msg{Environment.NewLine}ContentLength={msg.Length}{Environment.NewLine}Content={msg}";
			}
		}

		static string PrintProperties(object obj)
		{
			if (obj == null)
				return "NULL";

			var sb = new StringBuilder();

			var objType = obj.GetType();
			var properties = objType.GetProperties();
			foreach (var p in properties)
			{
				sb.AppendFormat("{1}: {2}", p.Name, p.ToString());
			}

			return sb.ToString();
		}
		#endregion

		#region EFTRequestToString

		public string EFTRequestToString(EFTRequest eftRequest)
		{
			// Build the request string.
			var request = BuildRequest(eftRequest);
			var len = request.Length + 5;
			request.Insert(0, '#');
			request.Insert(1, len.PadLeft(4));
			return request.ToString();
		}

		StringBuilder BuildRequest(EFTRequest eftRequest)
		{
			if (eftRequest is EFTLogonRequest)
			{
				return BuildEFTLogonRequest((EFTLogonRequest)eftRequest);
			}

			if (eftRequest is EFTTransactionRequest)
			{
				return BuildEFTTransactionRequest((EFTTransactionRequest)eftRequest);
			}

			if (eftRequest is EFTGetLastTransactionRequest)
			{
				return BuildEFTGetLastTransactionRequest((EFTGetLastTransactionRequest)eftRequest);
			}

			if (eftRequest is EFTReprintReceiptRequest)
			{
				return BuildEFTReprintReceiptRequest((EFTReprintReceiptRequest)eftRequest);
			}

			if (eftRequest is SetDialogRequest)
			{
				return BuildSetDialogRequest((SetDialogRequest)eftRequest);
			}

			if (eftRequest is EFTControlPanelRequest)
			{
				return BuildControlPanelRequest((EFTControlPanelRequest)eftRequest);
			}

			if (eftRequest is EFTSettlementRequest)
			{
				return BuildSettlementRequest((EFTSettlementRequest)eftRequest);
			}

			if (eftRequest is EFTStatusRequest)
			{
				return BuildStatusRequest((EFTStatusRequest)eftRequest);
			}

			if (eftRequest is EFTChequeAuthRequest)
			{
				return BuildChequeAuthRequest((EFTChequeAuthRequest)eftRequest);
			}

			if (eftRequest is EFTQueryCardRequest)
			{
				return BuildQueryCardRequest((EFTQueryCardRequest)eftRequest);
			}

			if (eftRequest is EFTGetPasswordRequest)
			{
				return BuildGetPasswordRequest((EFTGetPasswordRequest)eftRequest);
			}

			if (eftRequest is EFTSlaveRequest)
			{
				return BuildSlaveRequest((EFTSlaveRequest)eftRequest);
			}

			if (eftRequest is EFTConfigureMerchantRequest)
			{
				return BuildConfigMerchantRequest((EFTConfigureMerchantRequest)eftRequest);
			}

			if (eftRequest is EFTCloudLogonRequest)
			{
				return BuildCloudLogonRequest((EFTCloudLogonRequest)eftRequest);
			}

			if (eftRequest is EFTClientListRequest)
			{
				return BuildGetClientListRequest((EFTClientListRequest)eftRequest);
			}

			if (eftRequest is EFTSendKeyRequest)
			{
				return BuildSendKeyRequest((EFTSendKeyRequest)eftRequest);
			}

			if (eftRequest is EFTReceiptRequest)
			{
				return BuildReceiptRequest((EFTReceiptRequest)eftRequest);
			}

			if (eftRequest is EFTPayAtTableRequest)
			{
				return BuildPayAtTableRequest((EFTPayAtTableRequest)eftRequest);
			}

			if (eftRequest is EFTBasketDataRequest)
			{
				return BuildBasketDataRequest((EFTBasketDataRequest)eftRequest);
			}

			throw new Exception("Unknown EFTRequest type.");
		}

		StringBuilder BuildEFTTransactionRequest(EFTTransactionRequest v)
		{
			var r = new StringBuilder();
			r.Append("M");
			r.Append("0");
			r.Append(v.Merchant.PadRightAndCut(2));
			r.Append((char)v.TxnType);
			r.Append(v.TrainingMode ? '1' : '0');
			r.Append(v.EnableTip ? '1' : '0');
			r.Append(v.AmtCash.PadLeftAsInt(9));
			r.Append(v.AmtPurchase.PadLeftAsInt(9));
			r.Append(v.AuthCode.PadLeft(6));
			r.Append(v.TxnRef.PadRightAndCut(16));
			r.Append((char)v.ReceiptAutoPrint);
			r.Append((char)v.CutReceipt);
			r.Append((char)v.PanSource);
			r.Append(v.Pan.PadRightAndCut(20));
			r.Append(v.DateExpiry.PadRightAndCut(4));
			r.Append(v.Track2.PadRightAndCut(40));
			r.Append((char)v.AccountType);
			r.Append(v.Application.ToApplicationString());
			r.Append(v.RRN.PadRightAndCut(12));
			r.Append(v.CurrencyCode.PadRightAndCut(3));
			r.Append((char)v.OriginalTxnType);
			r.Append(v.Date != null ? v.Date.Value.ToString("ddMMyy") : "      ");
			r.Append(v.Time != null ? v.Time.Value.ToString("HHmmss") : "      ");
			r.Append(" ".PadRightAndCut(8)); // Reserved
			r.Append(v.PurchaseAnalysisData.GetAsString(true));

			return r;
		}

		StringBuilder BuildEFTLogonRequest(EFTLogonRequest v)
		{
			var r = new StringBuilder();
			r.Append("G");
			r.Append((char)v.LogonType);
			r.Append(v.Merchant.PadRightAndCut(2));
			r.Append((char)v.ReceiptAutoPrint);
			r.Append((char)v.CutReceipt);
			r.Append(v.Application.ToApplicationString());
			r.Append(v.PurchaseAnalysisData.GetAsString(true));
			return r;
		}

		StringBuilder BuildEFTReprintReceiptRequest(EFTReprintReceiptRequest v)
		{
			var r = new StringBuilder();
			r.Append("C");
			r.Append((char)v.ReprintType);
			r.Append(v.Application.ToApplicationString());
			r.Append((char)v.CutReceipt);
			r.Append((char)v.ReceiptAutoPrint);
			r.Append(v.Merchant.PadRightAndCut(2));
			r.Append(v.OriginalTxnRef.Length > 0 ? v.OriginalTxnRef.PadRightAndCut(16) : "");
			return r;
		}

		StringBuilder BuildEFTGetLastTransactionRequest(EFTGetLastTransactionRequest v)
		{
			var r = new StringBuilder();
			r.Append("N");
			r.Append("0");
			r.Append(v.Application.ToApplicationString());
			r.Append(v.Merchant.PadRightAndCut(2));
			r.Append(v.TxnRef.Length > 0 ? v.TxnRef.PadRightAndCut(16) : "");
			return r;
		}

		StringBuilder BuildSetDialogRequest(SetDialogRequest v)
		{
			var r = new StringBuilder();
			r.Append("2");
			r.Append(v.DisableDisplayEvents ? '5' : ' ');
			r.Append((char)v.DialogType);
			r.Append(v.DialogX.PadLeft(4));
			r.Append(v.DialogY.PadLeft(4));
			r.Append(v.DialogPosition.ToString().PadRightAndCut(12));
			r.Append(v.EnableTopmost ? '1' : '0');
			r.Append(v.DialogTitle.PadRightAndCut(32));
			return r;
		}

		StringBuilder BuildControlPanelRequest(EFTControlPanelRequest v)
		{
			var r = new StringBuilder();
			r.Append("5"); // ControlPanel
			r.Append((char)v.ControlPanelType);
			r.Append((char)v.ReceiptPrintMode);
			r.Append((char)v.ReceiptCutMode);
			r.Append((char)v.ReturnType);
			return r;
		}

		StringBuilder BuildSettlementRequest(EFTSettlementRequest v)
		{
			var r = new StringBuilder();
			r.Append("P");
			r.Append((char)v.SettlementType);
			r.Append(v.Merchant.PadRightAndCut(2));
			r.Append((char)v.ReceiptAutoPrint);
			r.Append((char)v.CutReceipt);
			r.Append(v.ResetTotals ? '1' : '0');
			r.Append(v.Application.ToApplicationString());
			r.Append(v.PurchaseAnalysisData.GetAsString(true));
			return r;
		}

		StringBuilder BuildQueryCardRequest(EFTQueryCardRequest v)
		{
			var r = new StringBuilder();
			r.Append("J");
			r.Append((char)v.QueryCardType);
			r.Append(v.Application.ToApplicationString());
			r.Append(v.Merchant.PadRightAndCut(2));
			r.Append(v.PurchaseAnalysisData.GetAsString(true));
			return r;
		}

		StringBuilder BuildConfigMerchantRequest(EFTConfigureMerchantRequest v)
		{
			var r = new StringBuilder();
			r.Append("10");
			r.Append(v.Merchant.PadRightAndCut(2));
			r.Append(v.AIIC.PadLeft(11));
			r.Append(v.NII.PadLeft(3));
			r.Append(v.Caid.PadRightAndCut(15));
			r.Append(v.Catid.PadRightAndCut(8));
			r.Append(v.Timeout.PadLeft(3));
			r.Append(v.Application.ToApplicationString());
			return r;
		}

		StringBuilder BuildStatusRequest(EFTStatusRequest v)
		{
			var r = new StringBuilder();
			r.Append("K");
			r.Append((char)v.StatusType);
			r.Append(v.Merchant.PadRightAndCut(2));
			r.Append(v.Application.ToApplicationString());
			return r;
		}

		StringBuilder BuildChequeAuthRequest(EFTChequeAuthRequest v)
		{
			var r = new StringBuilder();
			r.Append("H0");
			r.Append(v.Application.ToApplicationString());
			r.Append(' ');
			r.Append(v.BranchCode.PadRightAndCut(6));
			r.Append(v.AccountNumber.PadRightAndCut(14));
			r.Append(v.SerialNumber.PadRightAndCut(14));
			r.Append(v.Amount.PadLeftAsInt(9));
			r.Append((char)v.ChequeType);
			r.Append(v.ReferenceNumber.PadRightAndCut(12));

			return r;
		}

		StringBuilder BuildGetPasswordRequest(EFTGetPasswordRequest v)
		{
			var r = new StringBuilder();
			r.Append("X");
			r.Append((char)CommandType.GetPassword);
			r.Append(v.MinPasswordLength.PadLeft(2));
			r.Append(v.MaxPassworkLength.PadLeft(2));
			r.Append(v.Timeout.PadLeft(3));
			r.Append("0" + (char)v.PasswordDisplay);
			return r;
		}

		StringBuilder BuildSlaveRequest(EFTSlaveRequest v)
		{
			var r = new StringBuilder();
			r.Append("X");
			r.Append((char)CommandType.Slave);
			r.Append(v.RawCommand);

			return r;
		}

		StringBuilder BuildGetClientListRequest(EFTClientListRequest v)
		{
			var r = new StringBuilder();
			r.Append("Q0");
			return r;
		}

		StringBuilder BuildCloudLogonRequest(EFTCloudLogonRequest v)
		{
			var r = new StringBuilder();
			r.Append("A ");
			r.Append(v.ClientID.PadRightAndCut(16));
			r.Append(v.Password.PadRightAndCut(16));
			r.Append(v.PairingCode.PadRightAndCut(16));
			return r;
		}

		StringBuilder BuildSendKeyRequest(EFTSendKeyRequest v)
		{
			var r = new StringBuilder();
			r.Append("Y0");
			r.Append((char)v.Key);
			if (v.Key == EFTPOSKey.Authorise && v.Data != null)
			{
				r.Append(v.Data.PadRightAndCut(20));
			}

			return r;
		}

		StringBuilder BuildReceiptRequest(EFTReceiptRequest v)
		{
			return new StringBuilder("3 ");
		}

		StringBuilder BuildPayAtTableRequest(EFTPayAtTableRequest request)
		{
			var r = new StringBuilder();
			r.Append("X");
			r.Append((char)CommandType.PayAtTable);
			r.Append(request.Header);
			r.Append(request.Content);

			return r;
		}

		StringBuilder BuildBasketDataRequest(EFTBasketDataRequest request)
		{
			var jsonContent = "{}";

            if (request.Command is EFTBasketDataCommandCreate)
            {
                var c = request.Command as EFTBasketDataCommandCreate;
                // Serializer the Basket object to JSON
                using (var ms = new System.IO.MemoryStream())
                {
                    // Would be better to use use Newtonsoft.Json here, but we don't want the dependency, so we are stuck with DataContractJsonSerializer
                    // We need to add the possible known types from c.Basket.Items to the "known types" of the serializer otherwise it throws an exception
                    var serializerSettings = new DataContractJsonSerializerSettings()
                    {
                        EmitTypeInformation = System.Runtime.Serialization.EmitTypeInformation.Never,
                        IgnoreExtensionDataObject = false,
                        SerializeReadOnlyTypes = true,
                        KnownTypes = (c?.Basket?.Items?.Count > 0) ? c.Basket.Items.Select(bi => bi.GetType()).Distinct() : new List<Type>() { typeof(EFTBasketItem) }
                    };
                    var serializer = new DataContractJsonSerializer((c?.Basket != null) ? c.Basket.GetType() : typeof(EFTBasket), serializerSettings);
                    serializer.WriteObject(ms, c.Basket);
                    var json = ms.ToArray();
                    ms.Close();
                    jsonContent = System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
                }

            }
            else if (request.Command is EFTBasketDataCommandAdd)
            {
                var c = request.Command as EFTBasketDataCommandAdd;
                // TODO: We can only have one item in an "add" command. Should we validate this request??

                // Serializer the Basket object to JSON
                using (var ms = new System.IO.MemoryStream())
                {
                    // Would be better to use use Newtonsoft.Json here, but we don't want the dependency, so we are stuck with DataContractJsonSerializer
                    // We need to add the possible known types from c.Basket.Items to the "known types" of the serializer otherwise it throws an exception
                    var serializerSettings = new DataContractJsonSerializerSettings()
                    {
                        EmitTypeInformation = System.Runtime.Serialization.EmitTypeInformation.Never,
                        IgnoreExtensionDataObject = false,
                        SerializeReadOnlyTypes = true,
                        KnownTypes = (c?.Basket?.Items?.Count > 0) ? c.Basket.Items.Select(bi => bi.GetType()).Distinct() : new List<Type>() { typeof(EFTBasketItem) }
                    };
                    var serializer = new DataContractJsonSerializer((c?.Basket != null) ? c.Basket.GetType() : typeof(EFTBasket), serializerSettings);
                    serializer.WriteObject(ms, c.Basket);
                    var json = ms.ToArray();
                    ms.Close();
                    jsonContent = System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
                }
            }
            else if (request.Command is EFTBasketDataCommandDelete)
            {
                var c = request.Command as EFTBasketDataCommandDelete;
                // Build our fake basket
                var b = new EFTBasket()
                {
                    Id = c.BasketId,
                    Items = new List<EFTBasketItem>()
                        {
                            new EFTBasketItem()
                            {
                                Id = c.BasketItemId
                            }
                        }
                };

                // Serializer the Basket object to JSON
                using (var ms = new System.IO.MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(EFTBasket), new DataContractJsonSerializerSettings() { IgnoreExtensionDataObject = false, SerializeReadOnlyTypes = true });
                    serializer.WriteObject(ms, b);
                    var json = ms.ToArray();
                    ms.Close();
                    jsonContent = System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
                }
            }
            else if (request.Command is EFTBasketDataCommandRaw)
            {
                var c = request.Command as EFTBasketDataCommandRaw;
                jsonContent = c.BasketContent;					
			}

			var r = new StringBuilder();
			r.Append("X");
			r.Append((char)CommandType.BasketData);
			r.Append(jsonContent.Length.PadLeft(6));
			r.Append(jsonContent);
			return r;
		}


		#endregion

		public EFTResponse XMLStringToEFTResponse(string msg)
		{
			EFTPosAsPinpadResponse response = null;
			try
			{
				response = XMLSerializer.Deserialize<EFTPosAsPinpadResponse>(msg);
			}
			catch (Exception)
			{
			}

			return response;
		}

		public string EFTRequestToXMLString(EFTRequest eftRequest)
		{
            if (eftRequest is EFTPosAsPinpadRequest)
            {
                return EFTRequestToXMLString(eftRequest as EFTPosAsPinpadRequest);
            }
            else
            {
                return string.Empty;
            }
		}

		private string EFTRequestToXMLString(EFTPosAsPinpadRequest eftRequest)
		{
			try
			{
				var response = XMLSerializer.Serialize(eftRequest);
				return response.Insert(0, $"&{(response.Length + 7).ToString("000000")}");
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}

}
