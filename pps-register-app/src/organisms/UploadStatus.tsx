import React from 'react';

interface UploadStatusProps {
  totalRecords: number;
  invalidRecords: number;
  processedRecords: number;
  updatedRecords: number;
  addedRecords: number;
  isProcessing: boolean;
}

export const UploadStatus: React.FC<UploadStatusProps> = ({
  totalRecords,
  invalidRecords,
  processedRecords,
  updatedRecords,
  addedRecords,
  isProcessing,
}) => {
  const stats = [
    { label: 'Total Records', value: totalRecords },
    { label: 'Invalid Records', value: invalidRecords },
    { label: 'Processed Records', value: processedRecords },
    { label: 'Updated Records', value: updatedRecords },
    { label: 'Added Records', value: addedRecords },
  ];

  return (
    <div className="bg-white rounded-lg shadow p-6">
      <h3 className="text-lg font-semibold mb-4">
        {isProcessing ? 'Processing Upload...' : 'Upload Summary'}
      </h3>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
        {stats.map((stat) => (
          <div key={stat.label} className="p-4 bg-gray-50 rounded-lg">
            <div className="text-sm text-gray-600">{stat.label}</div>
            <div className="text-2xl font-bold text-gray-900">{stat.value}</div>
          </div>
        ))}
      </div>
    </div>
  );
};
