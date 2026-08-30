import { useLoaderData } from "react-router";
import type { SettingsDataResponse } from "../features/settings/types";
import { SettingsProfile } from "../features/settings/components/settingsProfile";
import { SettingsEmail } from "../features/settings/components/settingsEmail";
import { SettingsPassword } from "../features/settings/components/settingsPassword";


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
            
            <SettingsPassword />
        </>
    );
}