//
// Mono-specific additions to Microsoft's _SecureChannel.cs
//

namespace System.Net.Security {

    partial class SecureChannel
    {
        internal ProtocolToken CreateShutdownMessage()
        {
            var buffer = SSPIWrapper.CreateShutdownMessage(m_SecModule, m_SecurityContext);
            return new ProtocolToken(buffer, SecurityStatus.ContinueNeeded);
        }

        internal bool IsClosed {
            get {
                if (m_SecModule == null || m_SecurityContext == null || m_SecurityContext.IsClosed)
                    return true;
               return SSPIWrapper.IsClosed(m_SecModule, m_SecurityContext);
            }
        }
    }
}
