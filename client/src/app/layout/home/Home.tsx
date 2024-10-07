import React from 'react';
import { Link } from 'react-router-dom';
import { Container, Typography, Box, Button } from '@mui/material';

export default function Home() {
    return (
        <Container maxWidth="sm">
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'center',
                    height: '100vh',
                    gap: 2,
                }}
            >
                <Typography variant="h2" component="h1" gutterBottom>
                    Home Page
                </Typography>
                <Typography variant="body1" align="center">
                    Щоб побачити список urls, перейдіть у
                </Typography>
                <Button variant="contained" color="primary" component={Link} to="/urls">
                    Urls
                </Button>
            </Box>
        </Container>
    );
}