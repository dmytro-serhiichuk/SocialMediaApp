import { useState } from "react";
import type { SessionInfo, SessionState } from "../types";
import { apiClient, ApiError } from "../../../api/client";


export function Sessions({userSessions}: {userSessions: SessionInfo[]}) {
    const [sessions, setSessions] = useState<SessionInfo[]>(userSessions);
    const [busy, setBusy] = useState<boolean>(false);

    const handleSessionTermination = async (id: number) => {
        const session = sessions.find(x => x.id === id);
        if (!session) return;
        
        setBusy(true);

        const expectedErrors = [404, 409];

        try {
            await apiClient(`settings/sessions/${id}/terminate`, { method: "POST" });
            setSessions(sessions.map(x => x.id === id ? { ...x, state: "Terminated" } : x));
        } catch (err) {
            if (err instanceof ApiError && expectedErrors.includes(err.status)) {
                if (err.status === 409) alert("Something went wrong...");
                else setSessions(sessions.map(x => x.id === id ? { ...x, state: "Terminated" } : x));
            }
            else {
                throw err;
            }
        } finally {
            setBusy(false);
        }
    }

    const hadleTerminateAll = async () => {
        setBusy(true);

        try {
            await apiClient("settings/sessions/terminate", { method: "POST" });
            setSessions(sessions.map(x => x.state === "Active" && !x.isCurrentSession 
                ? { ...x, state: "Terminated" } 
                : x)
            );
        } catch (err) {
            alert("Something went wrong...");
        } finally {
            setBusy(false);
        }
    }

    const sessionStatus = (state: SessionState) => {
        switch (state) {
            case "LoggedOut":
                return "Logged Out";
            case "ReuseDetected":
                return "Suspicious Activity";
            default:
                return "Terminated";
        }
    }

    return (
        <>
            <h3>Sessions:</h3>

            <h4>Active Sessions:</h4>

            {sessions.map(s => s.state === "Active" && (
                <div key={s.id}>
                    <p>{s.deviceInfo}</p>
                    <p>Ip: {s.ip}</p>

                    <p>First login: {new Date(s.firstLogin).toISOString()}</p>
                    <p>Last login: {new Date(s.lastLogin).toISOString()}</p>

                    {s.isCurrentSession ? 
                        <>
                            <p>* Current Session</p>

                            {sessions.length > 1 && 
                                <button 
                                    onClick={hadleTerminateAll}
                                    disabled={busy}>
                                    Terminate All Other Sessions
                                </button>
                            }
                        </>
                    :
                        <button 
                            onClick={() => handleSessionTermination(s.id)}
                            disabled={busy}>
                            Terminate
                        </button>
                    }
                    
                    <hr />
                </div>
            ))}

            {sessions.some(x => x.state !== "Active") && (
                <h4>Inactive Sessions:</h4>
            )}
            {sessions.map(s => s.state !== "Active" && (
                <div key={s.id}>
                    <p>{s.deviceInfo}</p>
                    <p>Ip: {s.ip}</p>

                    <p>First login: {new Date(s.firstLogin).toISOString()}</p>
                    <p>Last login: {new Date(s.lastLogin).toISOString()}</p>

                    <p>Status: {sessionStatus(s.state)}</p>
                    
                    <hr />
                </div>
            ))}
        </>
    );
}