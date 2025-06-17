import React, {useState} from 'react';
import './App.css';
import axios from 'axios'

const App = () => {

    const [newsUrl, setNewsUrl] = useState('')
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const [newsSummary, setSummary] = useState('');
   
    const myComponentStyle = {
        background: 'linear-gradient(135deg, rgb(102, 126, 234) 0%, rgb(118, 75, 162) 100%)', 
        fontFamily: 'Poppins, sans-serif', 
        padding: '2rem',
    };

    const onUrlChange = (e) => {
        setNewsUrl(e.target.value)
    }


    const onSummarizeClick = async () => {
        if (!newsUrl) {
            setError('Please enter URL for a news article.');
            return;
        }

        setError('');
        setLoading(true);
        setSummary('');

        try {

            const { data } = await axios.post('/api/NewsSummarizer/Summarize', { newsUrl });
            console.log(data.newsSummary)

            setSummary(data.newsSummary);
        } catch (err) {
            setError('An error occurred while summarizing.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };
    
    
    return (
        <div className="min-vh-100 d-flex flex-column justify-content-center align-items-center text-white" style={myComponentStyle} >
            <h1 className="mb-4 fw-bold text-center">📰 AI Article Summarizer</h1>
            <div className="card p-4 shadow-lg rounded-4" style={{ maxWidth:'600px', width: '100%'}} >
                <div className="form-group mb-3">
                    <label className="form-label fw-semibold">Paste article URL:</label>
                    <input type="text" onChange={onUrlChange} className="form-control" placeholder="https://example.com/news/article" value={newsUrl}></input>
                </div>
                <button onClick={onSummarizeClick} className="btn btn-primary w-100 fw-bold" disabled={!newsUrl}> {loading ? (
                    <>
                        <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                        Summarizing...
                    </>
                ) : (
                    'Summarize Article'
                )}</button>
            </div>

            {!!newsSummary && (
                <div className="card mt-5 shadow-lg bg-light border-0 rounded-4">
                    <div className="card-header bg-dark text-white fw-bold rounded-top-4">
                       Summary
                    </div>
                    <div className="card-body">
                        <pre className="card-text text-dark" style={{ whiteSpace: 'pre-wrap' }}>{newsSummary}</pre>
                    </div>
                </div>
            )}
        </div>
    );
   
}

export default App;