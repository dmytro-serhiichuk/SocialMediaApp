import { useState } from "react";
import type { SessionInfo } from "../types";
import { apiClient, ApiError } from "../../../api/client";


export function Sessions({userSessions}: {userSessions: SessionInfo[]}) {
    const [sessions, setSessions] = useState<SessionInfo[]>(userSessions);
    const [busy, setBusy] = useState<boolean>(false);

    const handleSessionTermination = async (id: number) => {
        const session = sessions.find(x => x.id === id);
        if (!session) return;
        
        setBusy(true);

        try {
            await apiClient(`settings/sessions/${id}`, { method: "DELETE" });
            setSessions(sessions.filter(x => x.id !== id));
        } catch (err) {
            if (err instanceof ApiError && err.status === 404) {
                setSessions(sessions.filter(x => x.id !== id));
                return;
            }
            alert("Something went wrong...");
        } finally {
            setBusy(false);
        }
    }

    const hadleTerminateAll = async () => {
        setBusy(true);

        try {
            await apiClient("settings/sessions", { method: "DELETE" });
            setSessions(sessions.filter(x => x.isCurrentSession));
        } catch (err) {
            alert("Something went wrong...");
        } finally {
            setBusy(false);
        }
    }

    return (
        <>
            <h3>Sessions:</h3>

            {sessions.map(s => (
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
        </>
    );
}