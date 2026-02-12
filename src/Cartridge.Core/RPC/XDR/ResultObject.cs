using System;

namespace Cartridge.Core.RPC.XDR
{
    /// <summary>
    /// Represents an XDR result object that can contain either a success value of type TOK
    /// or a failure value of type TFAIL.
    /// </summary>
    public sealed class ResultObject<TOK, TFAIL> : IXdrData
        where TOK : IXdrData, new()
        where TFAIL : IXdrData, new()
    {
        public XdrStatus Status { get; set; }
        public TOK? OK { get; set; }
        public TFAIL? Fail { get; set; }

        public ResultObject()
        {
            Status = XdrStatus.OK;
            OK = new TOK();
            Fail = default;
        }

        public ResultObject(XdrStatus status, TOK? ok, TFAIL? fail)
        {
            Status = status;
            OK = ok;
            Fail = fail;
        }

        public static ResultObject<TOK, TFAIL> CreateSuccess(TOK value)
            => new(XdrStatus.OK, value, default);

        public static ResultObject<TOK, TFAIL> CreateFailure(XdrStatus status, TFAIL value)
            => new(status, default, value);

        public async ValueTask XdrEncodeAsync(XdrEncodingStream xdr)
        {
            await xdr.EncodeIntAsync((int)Status);

            if (Status == XdrStatus.OK)
            {
                if (OK == null)
                    throw new XdrException("Success status requires non-null OK value");
                await OK.XdrEncodeAsync(xdr);
            }
            else
            {
                if (Fail == null)
                    throw new XdrException("Failure status requires non-null Fail value");
                await Fail.XdrEncodeAsync(xdr);
            }
        }

        public async ValueTask XdrDecodeAsync(XdrDecodingStream xdr)
        {
            Status = (XdrStatus)await xdr.DecodeIntAsync();

            if (Status == XdrStatus.OK)
            {
                OK = new TOK();
                await OK.XdrDecodeAsync(xdr);
                Fail = default;
            }
            else
            {
                Fail = new TFAIL();
                await Fail.XdrDecodeAsync(xdr);
                OK = default;
            }
        }

        /// <summary>
        /// Executes different actions based on whether the result represents success or failure.
        /// </summary>
        public TResult Match<TResult>(
            Func<TOK, TResult> onSuccess,
            Func<XdrStatus, TFAIL, TResult> onFailure)
        {
            if (Status == XdrStatus.OK && OK != null)
                return onSuccess(OK);
            else if (Fail != null)
                return onFailure(Status, Fail);
            throw new InvalidOperationException("Invalid result state");
        }

        /// <summary>
        /// Executes different actions based on whether the result represents success or failure.
        /// </summary>
        public async Task<TResult> MatchAsync<TResult>(
            Func<TOK, Task<TResult>> onSuccess,
            Func<XdrStatus, TFAIL, Task<TResult>> onFailure)
        {
            if (Status == XdrStatus.OK && OK != null)
                return await onSuccess(OK);
            else if (Fail != null)
                return await onFailure(Status, Fail);
            throw new InvalidOperationException("Invalid result state");
        }
    }
}