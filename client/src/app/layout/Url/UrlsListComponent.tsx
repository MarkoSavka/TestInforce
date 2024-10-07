import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../features/AuthContext';
import Swal from 'sweetalert2';
import './UrlsListComponent.css';

interface Url {
  id: number;
  shortUrl: string;
  fullUrl: string;
  createdDate: string;
  createdById?: number; 
  createdBy?: undefined | null;
  isEditing: boolean;
  isNew: boolean; 
}

interface User {
  id: number;
  email: string;
  name: string;
  token: string;
  urls: Url[];
}

export default function UrlsListComponent() {
  const { isLoggedIn } = useAuth();
  const [localUrls, setLocalUrls] = useState<Url[]>([]);
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchCurrentUser = async () => {
      try {
        const response = await fetch('http://localhost:5000/api/User/currentUser', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });
        if (!response.ok) {
          throw new Error('Failed to fetch current user');
        }
        const data = await response.json();
        setCurrentUser(data);
      } catch (error) {
        console.error('Error fetching current user:', error);
      }
    };

    fetchCurrentUser();
  }, []);

  const fetchUrls = async () => {
    try {
      const response = await fetch('http://localhost:5000/api/url/get', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      if (!response.ok) {
        throw new Error('Failed to fetch URLs');
      }
      const data = await response.json();
      console.log('Fetched URLs:', data);
      setLocalUrls(data || []);
    } catch (error) {
      console.error('Failed to fetch URLs:', error);
    }
  };

  useEffect(() => {
    if (isLoggedIn) {
      fetchUrls();
    }
  }, [isLoggedIn]);

  useEffect(() => {
    localStorage.setItem('urls', JSON.stringify(localUrls));
  }, [localUrls]);

  const handleEditToggle = (index: number) => {
    const urlToEdit = localUrls[index];
    if (currentUser && urlToEdit.createdById !== currentUser.id) {
      Swal.fire({
        icon: 'error',
        title: 'Unauthorized',
        text: 'You cannot edit this URL.',
      });
      return;
    }
    const newUrls = [...localUrls];
    newUrls[index].isEditing = !newUrls[index].isEditing;
    setLocalUrls(newUrls);
  };

  const handleDelete = async (index: number) => {
    const urlToDelete = localUrls[index];
    try {
      const response = await fetch(`http://localhost:5000/api/url/delete?id=${urlToDelete.id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (response.status === 403) {
        console.error('You do not have permission to delete this URL.');
        return;
      }

      if (!response.ok) {
        throw new Error('Failed to delete URL');
      }

      const newUrls = localUrls.filter((_, i) => i !== index);
      setLocalUrls(newUrls);
    } catch (error) {
      console.error('Failed to delete URL:', error);
    }
  };

  const handleShortUrlChange = (index: number, newShortUrl: string) => {
    const newUrls = [...localUrls];
    newUrls[index].shortUrl = newShortUrl;
    setLocalUrls(newUrls);
  };

  const handleFullUrlChange = (index: number, newFullUrl: string) => {
    const newUrls = [...localUrls];
    newUrls[index].fullUrl = newFullUrl;
    setLocalUrls(newUrls);
  };

  const handleSubmit = async (index: number) => {
    const urlToSubmit = localUrls[index];
    const urlDto = {
      id: 0,
      shortUrl: urlToSubmit.shortUrl.trim(), 
      fullUrl: urlToSubmit.fullUrl.trim(),
      createdDate: urlToSubmit.createdDate
    };

    if (!urlDto.fullUrl) {
      console.error('FullUrl is required.');
      console.log('FullUrl:', urlDto.fullUrl);
      return;
    }

    console.log('Submitting URL:', urlDto);

    try {
      const response = await fetch('http://localhost:5000/api/Url/add', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(urlDto) 
      });

      if (!response.ok) {
        const errorData = await response.text(); 
        console.error('Failed to submit URL:', errorData);
        throw new Error('Failed to submit URL');
      }

      const data = await response.json();
      const newUrls = [...localUrls];
      newUrls[index] = { ...newUrls[index], id: data.id, isNew: false, isEditing: false }; 
      setLocalUrls(newUrls);

      console.log('URL submitted successfully:', urlDto);
    } catch (error) {
      console.error('Failed to submit URL:', error);
    }
  };

  const handleConfirm = async (index: number) => {
    const urlToSubmit = localUrls[index];
    const urlDto = {
      id: urlToSubmit.id || 0, 
      shortUrl: urlToSubmit.shortUrl.trim(),
      fullUrl: urlToSubmit.fullUrl.trim(), 
      createdDate: urlToSubmit.createdDate
    };

    if (!urlDto.fullUrl) {
      console.error('FullUrl is required.');
      console.log('FullUrl:', urlDto.fullUrl);
      return;
    }

    console.log('Confirm URL:', urlDto);

    try {
      console.log('Starting fetch request to update URL:', urlDto); 

      const response = await fetch('http://localhost:5000/api/Url/update', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(urlDto) 
      });

      console.log('Fetch request completed'); 

      if (!response.ok) {
        const errorData = await response.text(); 
        console.error('Failed to confirm URL:', errorData);
        throw new Error(`Failed to confirm URL: ${response.status} ${response.statusText}`);
      }

      const data = await response.json();
      console.log('Response data:', data); 

      const newUrls = [...localUrls];
      newUrls[index] = { ...newUrls[index], isNew: false, isEditing: false }; 
      setLocalUrls(newUrls);

      console.log('URL confirmed successfully:', data); 
    } catch (error) {
      console.error('Failed to confirm URL:', error);
    }
  };

  const handleGenerate = async (index: number) => {
    const urlToGenerate = localUrls[index];
    const cleanedShortUrl = urlToGenerate.shortUrl.replace("https://", "").replace(".com", "").replace("http://", "").trim();

    if (!cleanedShortUrl) {
      console.error('ShortUrl is required and cannot be empty after cleaning.');
      return;
    }

    const urlDto = {
      id: urlToGenerate.id || 0,
      shortUrl: cleanedShortUrl,
      fullUrl: urlToGenerate.fullUrl.trim(),
      createdDate: urlToGenerate.createdDate
    };

    if (!urlDto.fullUrl) {
      console.error('FullUrl is required.');
      return;
    }

    try {
      const response = await fetch('http://localhost:5000/api/url/generate', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(urlDto)
      });
      if (!response.ok) {
        const errorData = await response.json();
        console.error('Failed to generate URL:', errorData);
        throw new Error('Failed to generate URL');
      }
      const data = await response.json();
      if (!data.shortUrl.trim()) {
        console.error('Generated shortUrl is empty.');
        throw new Error('Generated shortUrl is empty.');
      }
      const newUrls = [...localUrls];
      newUrls[index] = { ...newUrls[index], shortUrl: data.shortUrl };
      setLocalUrls(newUrls);
    } catch (error) {
      console.error('Failed to generate URL:', error);
    }
  };

  const handleAddNew = () => {
    if (!currentUser) {
      console.error('User is not defined');
      return;
    }

    console.log('Current user ID:', currentUser.id);

    const newUrl: Url = {
      id: Date.now(),
      shortUrl: '',
      fullUrl: '',
      createdDate: new Date().toISOString(),
      createdById: currentUser.id,
      createdBy: null,
      isEditing: true,
      isNew: true
    };
    setLocalUrls((prevUrls) => [...prevUrls, newUrl]);
  };

  const handleLoginRedirect = () => {
    navigate('/login');
  };

  const renderUrlRow = (url: Url, index: number) => {
    console.log('URL createdById:', url.createdById, 'Current user ID:', currentUser?.id);
    return (
      <tr key={index}>
        <td>
          <input
            type="text"
            value={url.fullUrl}
            onChange={(e) => handleFullUrlChange(index, e.target.value)}
            disabled={!url.isEditing}
          />
        </td>
        <td>
          <input
            type="text"
            value={url.shortUrl}
            onChange={(e) => handleShortUrlChange(index, e.target.value)}
            disabled={!url.isEditing}
          />
        </td>
        <td>
          {url.isNew ? (
            <button onClick={() => handleSubmit(index)}>Submit</button>
          ) : (
            <>
              <button onClick={() => url.isEditing ? handleConfirm(index) : handleEditToggle(index)}>
                {url.isEditing ? 'Confirm' : 'Edit'}
              </button>
              <button onClick={() => handleDelete(index)}>Delete</button>
              <button onClick={() => handleGenerate(index)}>Generate</button>
            </>
          )}
        </td>
        <td>
          {currentUser && url.createdById === currentUser.id ? 'My URL' : 'Other URL'}
        </td>
      </tr>
    );
  };

  return (
    <div className="urls-list">
      {!isLoggedIn ? (
        <div>
          <p>Для відображення всіх посилань увійдіть</p>
          <button onClick={handleLoginRedirect}>Login</button>
        </div>
      ) : (
        <div>
          <button onClick={handleAddNew}>Add New</button>
          <table>
            <thead>
              <tr>
                <th>Full URL</th>
                <th>Short URL</th>
                <th>Actions</th>
                <th>Owner</th>
              </tr>
            </thead>
            <tbody>
              {localUrls && localUrls.length > 0 ? (
                localUrls.map((url, index) => renderUrlRow(url, index))
              ) : (
                <tr>
                  <td colSpan={4}>No URLs available</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}