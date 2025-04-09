import { useState } from "react";
import Notification from "./Notification";

const useNotification = () => {
  const [notification, setNotification] = useState<Notification | null>(null);

  return { notification, updateNotification: setNotification };
};

export default useNotification;

