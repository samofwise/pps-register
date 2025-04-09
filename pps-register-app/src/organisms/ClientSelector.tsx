import React, { useState, useEffect } from 'react';
import { storageService } from '../helpers/localStorageService';
import personalPropertySecuritiesApi from '../api/personalPropertySecuritiesApi';
import { Button } from '../atoms/Button';

const ClientSelector = ({ className }: Props) => {
  const [selectedClient, setSelectedClient] = useState<number>(1);
  const invalidateUploads =
    personalPropertySecuritiesApi.useInvalidateUploads();
  const deleteAll = personalPropertySecuritiesApi.useDeleteAll();

  useEffect(() => {
    const storedClient = storageService.clientId.get();
    if (storedClient) setSelectedClient(storedClient);
  }, []);

  const handleDeleteAll = async () => {
    if (confirm('Are you sure?')) {
      await deleteAll.mutateAsync();
      invalidateUploads();
    }
  };

  const handleClientChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const clientId = parseInt(e.target.value);
    setSelectedClient(clientId);
    storageService.clientId.set(clientId);
    invalidateUploads();
  };

  return (
    <section className={`flex items-center gap-4 ${className}`}>
      <Button variant="outline" onClick={handleDeleteAll}>
        Clear Data
      </Button>
      <article className="flex items-center">
        <label
          htmlFor="client-select"
          className="flex text-sm font-medium text-gray-700"
        >
          Current:
        </label>
        <select
          id="client-select"
          value={selectedClient}
          onChange={handleClientChange}
          className="flex w-full text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
        >
          {Array.from({ length: 10 }, (_, i) => i + 1).map((clientId) => (
            <option key={clientId} value={clientId}>
              Client {clientId}
            </option>
          ))}
        </select>
      </article>
    </section>
  );
};

export default ClientSelector;

type Props = {
  className?: string;
};
