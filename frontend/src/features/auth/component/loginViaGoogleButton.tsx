import { API_URL } from "../../../constants";

export default function LoginViaGoogleButton() {
    const handleClick = () => {
        window.location.href = `${API_URL}auth/google`;
    }
    
    return (
        <button type="button" onClick={handleClick}>
            Login via Google
        </button>
    );
}