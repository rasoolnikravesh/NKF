// using NKP.Services.BaseData.Domain.SeedWorks;
//
// namespace NKP.Services.BaseData.Domain.SharedKernels.ValueObjects
// {
// 	public class EmailAddress : ValueObject
// 	{
// 		#region Constant(s)
// 		public const int MaxLength = 250;
//
// 		public const int VerificationKeyMaxLength = 32;
// 		#endregion /Constant(s)
//
// 		#region Static Member(s)
// 		public static FluentResults.Result<EmailAddress> Create(string value)
// 		{
// 			var result =
// 				new FluentResults.Result<EmailAddress>();
//
// 			value =
// 				Dtx.String.Fix(text: value);
//
// 			if (value is null)
// 			{
// 				string errorMessage = string.Format
// 					(Resources.Messages.Validations.Required,
// 					Resources.DataDictionary.EmailAddress);
//
// 				result.WithError(errorMessage: errorMessage);
//
// 				return result;
// 			}
//
// 			if (value.Length > MaxLength)
// 			{
// 				string errorMessage = string.Format
// 					(Resources.Messages.Validations.MaxLength,
// 					Resources.DataDictionary.EmailAddress, MaxLength);
//
// 				result.WithError(errorMessage: errorMessage);
//
// 				return result;
// 			}
//
// 			try
// 			{
// 				var emailAddress =
// 					new System.Net.Mail.MailAddress(value).Address;
// 			}
// 			catch
// 			{
// 				string errorMessage = string.Format
// 					(Resources.Messages.Validations.RegularExpression,
// 					Resources.DataDictionary.EmailAddress);
//
// 				result.WithError(errorMessage: errorMessage);
//
// 				return result;
// 			}
//
// 			var returnValue =
// 				new EmailAddress(value: value);
//
// 			result.WithValue(value: returnValue);
//
// 			return result;
// 		}
//
// 		public string GetVerificationKey()
// 		{
// 			string result =
// 				System.Guid.NewGuid().ToString().Replace("-", string.Empty);
//
// 			return result;
// 		}
// 		#endregion /Static Member(s)
//
// 		private EmailAddress() : base()
// 		{
// 		}
//
// 		private EmailAddress(string value) : this()
// 		{
// 			Value = value;
// 			IsVerified = false;
// 			VerificationKey = GetVerificationKey();
//
// 		}
//
// 		private EmailAddress
// 			(string value, bool isVerified, string verificationKey) : this(value: value)
// 		{
// 			IsVerified = isVerified;
// 			VerificationKey = verificationKey;
// 		}
//
// 		public string Value { get; }
//
// 		public bool IsVerified { get; }
//
// 		public string VerificationKey { get; }
//
// 		protected override
// 			System.Collections.Generic.IEnumerable<object> GetEqualityComponents()
// 		{
// 			yield return Value;
// 			yield return IsVerified;
// 			yield return VerificationKey;
// 		}
//
// 		public FluentResults.Result<EmailAddress> Verify()
// 		{
// 			var result =
// 				new FluentResults.Result<EmailAddress>();
//
// 			if (IsVerified == true)
// 			{
// 				// نشانی پست الکترونیکی قبلا تایید شده است
// 				result.WithError
// 					(errorMessage: Resources.Messages.Errors.Message0002);
//
// 				return result;
// 			}
//
// 			// Read Only!
// 			//IsVerified = true;
//
// 			var newObject =
// 				new EmailAddress
// 				(value: Value, isVerified: true, verificationKey: VerificationKey);
//
// 			result.WithValue(value: newObject);
//
// 			// نشانی پست الکترونیکی با موفقیت تایید گردید
// 			result.WithSuccess
// 				(successMessage: Resources.Messages.Successes.Message0002);
//
// 			return result;
// 		}
//
// 		public FluentResults.Result<EmailAddress> VerifyByKey(string verificationKey)
// 		{
// 			var result =
// 				new FluentResults.Result<EmailAddress>();
//
// 			if (string.Compare(VerificationKey, verificationKey, ignoreCase: false) != 0)
// 			{
// 				// کد فعال‌سازی صحیح نمی‌باشد
// 				result.WithError
// 					(errorMessage: Resources.Messages.Errors.Message0003);
//
// 				return result;
// 			}
//
// 			result = Verify();
//
// 			return result;
// 		}
//
// 		public override string ToString()
// 		{
// 			return Value;
// 		}
// 	}
// }
