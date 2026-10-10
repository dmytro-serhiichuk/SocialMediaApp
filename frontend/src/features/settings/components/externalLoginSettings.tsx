import { EXTERNAL_LOGIN_PROVIDERS } from "../types";
import ExternalLoginProvider from "./externalLoginProvider";

type ExternalLoginSettingsProps = {
    connectedProviders: string[],
    connectionError: string | null
}

export default function ExternalLoginSettings ({connectedProviders, connectionError}: ExternalLoginSettingsProps) {
    return (
        <>
            <h3>Login Providers:</h3>

            {connectionError && (<p>{connectionError}</p>)}

            {EXTERNAL_LOGIN_PROVIDERS.map((login, index) => (
                <ExternalLoginProvider
                    key={index}
                    provider={login}
                    isConnected={connectedProviders.includes(login.name)} />
            ))}
        </>
    );
}