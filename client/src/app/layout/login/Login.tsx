import React, { useState } from 'react';
import { TextField, Button, Container, Typography, Box } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../features/AuthContext';

export default function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [emailError, setEmailError] = useState('');
    const [loginError, setLoginError] = useState('');
    const navigate = useNavigate();
    const { login } = useAuth();

    const handleSubmit = (event: React.FormEvent) => {
        event.preventDefault();
        if (!validateEmail(email)) {
            setEmailError('Invalid email address');
            return;
        }
        
        fetch('http://localhost:5000/api/User/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({ email, password })
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Login failed');
                }
                return response.json();
            })
            .then(data => {
                localStorage.setItem('token', data.token);
                login(data.urls); 
                console.log(data.token);
                console.log(data.name);
                console.log(data.email);
                navigate('/urls');
            })
            .catch(error => {
                console.error('Error logging in:', error);
                setLoginError('Login failed. Please check your credentials and try again.');
            });
    };

    const validateEmail = (email: string) => {
        const re = /^[^\s@]+@[^\s@]+$/;
        return re.test(String(email).toLowerCase());
    };

    const handleRegisterRedirect = () => {
        navigate('/register');
    };

    return (
        <Container maxWidth="sm">
            <Box
                component="form"
                onSubmit={handleSubmit}
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    gap: 2,
                    mt: 4,
                }}
            >
                <Typography variant="h4" component="h1" gutterBottom>
                    Login
                </Typography>
                <TextField
                    label="User email"
                    variant="outlined"
                    type="email"
                    value={email}
                    onChange={(e) => {
                        setEmail(e.target.value);
                        setEmailError('');
                    }}
                    error={!!emailError}
                    helperText={emailError}
                    required
                />
                <TextField
                    label="Password"
                    variant="outlined"
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                {loginError && (
                    <Typography color="error">{loginError}</Typography>
                )}
                <Button type="submit" variant="contained" color="primary">
                    Login
                </Button>
                <Button variant="text" color="secondary" onClick={handleRegisterRedirect}>
                    Register, if you don't have account
                </Button>
            </Box>
        </Container>
    );
}