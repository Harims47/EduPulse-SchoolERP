import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store';
import AppRoutes from './routes/AppRoutes';
import GlobalLoader from './components/feedback/GlobalLoader';
import ToastContainer from './components/feedback/ToastContainer';

function App() {
  return (
    <Provider store={store}>
      <BrowserRouter>
        <GlobalLoader />
        <ToastContainer />
        <AppRoutes />
      </BrowserRouter>
    </Provider>
  );
}

export default App;
