import clsx from 'clsx';
import { useEffect, useState } from 'react';
import { CheckIcon, XMarkIcon } from '@heroicons/react/24/outline';
import Notification from './Notification';

export const InlineNotification = ({
  notification,
  duration = 3000,
  className,
  position = 'bottom-right',
}: InlineNotificationProps) => {
  const [isVisible, setIsVisible] = useState(true);
  const { type, message } = notification || {};

  useEffect(() => {
    if (notification) {
      setIsVisible(true);
      const timer = setTimeout(() => {
        setIsVisible(false);
      }, duration);

      return () => clearTimeout(timer);
    }
  }, [duration, notification]);

  useEffect(() => {
    const timer = setTimeout(() => {
      setIsVisible(false);
    }, duration);

    return () => clearTimeout(timer);
  }, [duration]);

  return (
    <article
      className={clsx(
        className,
        'absolute flex items-center gap-1 m-2',
        'text-sm transition-opacity ease-in-out',
        isVisible ? 'opacity-100 duration-700' : 'opacity-0 duration-2000',
        getTextColor(type),
        getPositionClass(position)
      )}
    >
      {type === 'success' && <CheckIcon className="w-4 h-4" />}
      {type === 'error' && <XMarkIcon className="w-4 h-4" />}
      <span>{message}</span>
    </article>
  );
};

type InlineNotificationProps = {
  className?: string;
  notification: Notification | null;
  duration?: number;
  position?: 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right';
};

const getTextColor = (type: Notification['type']) => {
  switch (type) {
    case 'success':
      return 'text-primary-500';
    case 'error':
      return 'text-red-500';
    default:
      return 'text-gray-500';
  }
};

const getPositionClass = (position: InlineNotificationProps['position']) => {
  switch (position) {
    case 'top-left':
      return 'top-0 left-0';
    case 'top-right':
      return 'top-0 right-0';
    case 'bottom-left':
      return 'bottom-0 left-0';
    case 'bottom-right':
      return 'bottom-0 right-0';
    default:
      return 'bottom-0 right-0';
  }
};
