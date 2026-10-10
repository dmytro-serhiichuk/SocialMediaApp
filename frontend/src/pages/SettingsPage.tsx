import { useLoaderData } from "react-router";
import { SettingsProfile } from "../features/settings/components/settingsProfile";
import { SettingsEmail } from "../features/settings/components/settingsEmail";
import { SettingsPassword } from "../features/settings/components/settingsPassword";
import { Sessions } from "../features/settings/components/sessions";
import SettingsPasswordCreate from "../features/settings/components/settingsPasswordCreate";
import ExternalLoginSettings from "../features/settings/components/externalLoginSettings";
import type { settingsLoader } from "../features/settings/loaders";


export default function SettingsPage() {
    const { data, providerConnectionError } = useLoaderData<typeof settingsLoader>();

    return (
        <>
            <h1>Settings</h1>

            <hr />

            <SettingsProfile 
                userName={data.userName}
                userTag={data.userTag} />

            <hr />
            
            <SettingsEmail 
                email={data.email}/>

            <hr />

            {data.hasPassword ?
                <SettingsPassword /> :
                <SettingsPasswordCreate email={data.email} />
            }

            <hr />

            <ExternalLoginSettings 
                connectedProviders={data.connectedLoginProviders}
                connectionError={providerConnectionError} />

            <hr />

            <Sessions userSessions={data.sessions} />
        </>
    );
}