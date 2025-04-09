import { useCallback, DragEventHandler, DragEvent, useMemo } from 'react';
import { FileRejection, useDropzone } from 'react-dropzone';
import { Button } from './Button';
import clsx from 'clsx';
import useIsWindowDragging from '../hooks/useIsWindowDragging';

const FileUpload = ({
  onFileSelected,
  onDrop,
  onDropRejected,
  isUploading,
}: FileUploadProps) => {
  const isWindowDragging = useIsWindowDragging();

  const dropInDropzone = useCallback(
    (acceptedFiles: File[]) => {
      const file = acceptedFiles[0];
      if (file && file.type === 'text/csv') {
        onFileSelected?.(file);
      }
    },
    [onFileSelected]
  );

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop: dropInDropzone,
    accept: {
      'text/csv': ['.csv'],
    },
    maxFiles: 1,
    multiple: false,
    disabled: isUploading,
    onDropRejected,
  });

  const rootProps = useMemo(() => getRootProps(), [getRootProps]);

  const handleDrop = (e: DragEvent<HTMLElement>) => {
    e.preventDefault();
    onDrop?.(e);
    rootProps.onDrop?.(e);
  };

  return (
    <article
      {...rootProps}
      onDrop={handleDrop}
      className={clsx(
        'border-2 border-dashed rounded-lg p-8 text-center transition-colors cursor-pointer text-primary-500',
        isDragActive || isWindowDragging
          ? 'border-current bg-[color-mix(in_srgb,currentColor_10%,white)]'
          : 'border-gray-300',
        'hover:bg-current-50'
      )}
    >
      <input {...getInputProps()} />
      <div className="flex flex-col gap-2 text-gray-600">
        <p className="text-lg font-medium">
          {isDragActive || isWindowDragging
            ? 'Drop the CSV file here'
            : 'Drag and drop your CSV file here'}
        </p>
        <p className="text-sm">or</p>
        <Button
          className="self-center"
          variant="outline"
          disabled={isUploading}
        >
          {isUploading ? 'Uploading...' : 'Select File'}
        </Button>
      </div>
    </article>
  );
};

export default FileUpload;

interface FileUploadProps {
  onFileSelected?: (file: File) => void;
  onDrop?: DragEventHandler<HTMLElement>;
  onDropRejected?: (fileRejections: FileRejection[]) => void;
  isUploading?: boolean;
}
