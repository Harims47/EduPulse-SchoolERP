import React from 'react';
import { useSelector } from 'react-redux';

const GlobalLoader = () => {
  const apiLoadingCount = useSelector((state) => state.ui.apiLoadingCount);

  if (apiLoadingCount === 0) return null;

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      height: '3px',
      backgroundColor: 'rgba(99, 102, 241, 0.15)',
      zIndex: 9999,
      overflow: 'hidden'
    }}>
      <div 
        style={{
          height: '100%',
          backgroundColor: 'var(--color-primary)',
          width: '50%',
          animation: 'loadProgress 1.5s infinite ease-in-out',
          borderRadius: 'var(--radius-full)'
        }} 
      />
      
      {/* Dynamic Keyframe style helper */}
      <style dangerouslySetInnerHTML={{__html: `
        @keyframes loadProgress {
          0% {
            transform: translateX(-100%);
            width: 30%;
          }
          50% {
            width: 60%;
          }
          100% {
            transform: translateX(200%);
            width: 30%;
          }
        }
      `}} />
    </div>
  );
};

export default GlobalLoader;
