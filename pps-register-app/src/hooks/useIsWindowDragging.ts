import { useState, useEffect } from 'react';

const useIsWindowDragging = () => {
  const [isWindowDragging, setIsWindowDragging] = useState(false);

  useEffect(() => {
    const start = (e: DragEvent) => {
      e.preventDefault();
      setIsWindowDragging(true);
    };
    const leave = (e: DragEvent) => {
      e.preventDefault();
      if (!e.relatedTarget) setIsWindowDragging(false);
    };
    const drop = (e: DragEvent) => {
      e.preventDefault();
      setIsWindowDragging(false);
    };

    window.addEventListener('dragenter', start);
    window.addEventListener('dragleave', leave);
    window.addEventListener('dragend', drop);
    window.addEventListener('drop', drop);

    return () => {
      window.removeEventListener('dragenter', start);
      window.removeEventListener('dragleave', leave);
      window.removeEventListener('dragend', drop);
      window.removeEventListener('drop', drop);
    };
  }, []);

  return isWindowDragging;
};

export default useIsWindowDragging;
