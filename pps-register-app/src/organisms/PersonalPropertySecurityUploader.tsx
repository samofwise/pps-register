import { DragEvent } from 'react';
import personalPropertySecuritiesApi from '../api/personalPropertySecuritiesApi';
import FileUpload from '../atoms/FileUpload';
import { getFile } from '../utils/fileUtils';
import { InlineNotification } from '../modules/notifications/InlineNotification';
import useNotification from '../modules/notifications/useNotification';
import { AxiosError } from 'axios';
import { FileRejection } from 'react-dropzone';

const PersonalPropertySecurityUploader = () => {
  const { mutateAsync, isPending } = personalPropertySecuritiesApi.useUpload();
  const { notification, updateNotification } = useNotification();

  const invalidateUploads =
    personalPropertySecuritiesApi.useInvalidateUploads();

  const invalidateValues =
    personalPropertySecuritiesApi.useInvalidatePersonalPropertySecurities();

  const uploadFile = async (file: File) => {
    try {
      await mutateAsync(file);
      invalidateUploads();
      invalidateValues();
      updateNotification({
        message: 'File uploaded successfully',
        type: 'success',
      });
    } catch (error) {
      if (error instanceof AxiosError) {
        const message = getErrorMessage(error);
        updateNotification({
          message,
          type: 'error',
        });
      }
    }
  };

  const handleDrop = async (e: DragEvent<HTMLElement>) => {
    e.preventDefault();
    const url = e.dataTransfer.getData('text/plain');
    if (url) {
      const file = await getFile(url);
      uploadFile(file);
    }
  };

  const handleDropRejected = (fileRejections: FileRejection[]) => {
    updateNotification({
      message: fileRejections[0].errors[0].message,
      type: 'error',
    });
  };

  return (
    <div className="relative">
      <FileUpload
        onFileSelected={uploadFile}
        onDrop={handleDrop}
        isUploading={isPending}
        onDropRejected={handleDropRejected}
      />
      <InlineNotification notification={notification} />
    </div>
  );
};

export default PersonalPropertySecurityUploader;

const getErrorMessage = (error: AxiosError): string => {
  if (error.response?.status === 500) {
    return 'An error occurred while processing the file. Please try again.';
  }
  return (
    (error.response?.data as string) ??
    'An error occurred while processing the file. Please try again.'
  );
};
