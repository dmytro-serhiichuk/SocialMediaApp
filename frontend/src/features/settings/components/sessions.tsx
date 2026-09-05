import { useState } from "react";
import type { SessionInfo } from "../types";


export function Sessions({userSessions}: {userSessions: SessionInfo[]}) {
    const [sessions, setSessions] = useState<SessionInfo[]>(userSessions);

    return (
        <>
            <h3>Sessions:</h3>

            {sessions.map(s => (
                <div key={s.id}>
                    <p>{s.deviceInfo}</p>
                    <p>Ip: {s.ip}</p>

                    <p>First login: {new Date(s.firstLogin).toISOString()}</p>
                    <p>Last login: {new Date(s.lastLogin).toISOString()}</p>

                    <hr />
                </div>
            ))}
        </>
    );
}