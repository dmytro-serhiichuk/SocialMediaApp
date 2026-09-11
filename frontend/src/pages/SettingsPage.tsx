import { useLoaderData } from "react-router";
import type { SettingsDataResponse } from "../features/settings/types";
import { SettingsProfile } from "../features/settings/components/settingsProfile";
import { SettingsEmail } from "../features/settings/components/settingsEmail";
import { SettingsPassword } from "../features/settings/components/settingsPassword";
import { Sessions } from "../features/settings/components/sessions";
import SettingsPasswordCreate from "../features/settings/components/settingsPasswordCreate";


export default function SettingsPage() {
    const settingsData = useLoaderData<SettingsDataResponse>();

    return (
        <>
            <h1>Settings</h1>

            <hr />

            <SettingsProfile 
                userName={settingsData.userName}
                userTag={settingsData.userTag} />

            <hr />
            
            <SettingsEmail 
                email={settingsData.email}/>

            <hr />

            {settingsData.hasPassword ?
                <SettingsPassword /> :
                <SettingsPasswordCreate email={settingsData.email} />
            }

            <hr />

            <Sessions userSessions={settingsData.sessions} />
        </>
    );
}