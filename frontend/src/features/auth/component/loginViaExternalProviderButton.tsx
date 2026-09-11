import type { ExternalLoginProvider } from "../../settings/types";

type LoginViaExternalProviderButtonProps = {
    provider: ExternalLoginProvider
}

export default function LoginViaExternalProviderButton({provider}: LoginViaExternalProviderButtonProps) {
    const handleClick = () => {
        window.location.href = provider.signInEndpoint;
    }
    
    return (
        <button type="button" onClick={handleClick}>
            Login via {provider.name}
        </button>
    );
}