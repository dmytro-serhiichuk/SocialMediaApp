import type { ExternalLoginProvider } from "../types";

type ExternalLoginProviderProps = {
    provider: ExternalLoginProvider,
    isConnected: boolean
}

export default function ExternalLoginProvider({provider, isConnected}: ExternalLoginProviderProps) {
    const handleConnect = () => {
        window.location.href = provider.connectEndpoint;
    }

    return (
        <div>
            <p>{provider.name}</p>
            {isConnected ?
                <p>Connected</p> :
                <button 
                    type="button"
                    onClick={handleConnect}>
                    Connect
                </button>
            }
            <hr />
        </div>
    );
}