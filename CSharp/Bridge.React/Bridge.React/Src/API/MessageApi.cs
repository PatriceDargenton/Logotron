
using System;
using System.Threading.Tasks;

namespace Bridge.React.Logotron.API
{
    public sealed class MessageApi : IReadAndWriteMessages
    {
        public Task SaveMessage(MessageDetails message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (string.IsNullOrWhiteSpace(message.Title))
                throw new ArgumentException("A title value must be provided");
            if (string.IsNullOrWhiteSpace(message.Content))
                throw new ArgumentException("A content value must be provided");

            return Task.Delay(250); // Simulate a roundtrip to the server
        }

        public Task SaveMessage(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("A content value must be provided");

            // Si on retourne directement, on ne peut pas effacer le texte saisi : trop rapide
            return Task.Delay(250); // Simulate a roundtrip to the server
        }

        public Task Attendre(int iDelaiMsec)
        {
            // Si on retourne directement, on ne peut pas effacer le texte saisi : trop rapide
            return Task.Delay(iDelaiMsec); // Simulate a roundtrip to the server
        }
    }
}