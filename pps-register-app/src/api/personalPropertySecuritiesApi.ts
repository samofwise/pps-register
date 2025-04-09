import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import PersonalPropertySecurityUpload from "../models/RegistrationSummary";
import api from "./axiosConfig";
import delay from "delay";


const uploadsKey = 'personalPropertySecurityUploads'

const personalPropertySecuritiesApi = {
  useGetUploads: () => useQuery({
    queryKey: [uploadsKey],
    queryFn: getUploads
  }),

  useInvalidateUploads: () => {
    const queryClient = useQueryClient()
    return () => queryClient.invalidateQueries({ queryKey: [uploadsKey] })
  },


  useUpload: () => useMutation({
    mutationFn: uploadFile,
  }),

  useDeleteAll: () => useMutation({
    mutationFn: deleteAll,
  }),
};

const getUploads = async () => {
  const promise = api.get<PersonalPropertySecurityUpload[]>('/personal-property-securities/uploads');
  const [response] = await Promise.all([promise, delay(300)]);
  return response.data;
};

const uploadFile = async (file: File) => {
  const formData = new FormData();
  formData.append('file', file);
  const response = await api.post('/personal-property-securities', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
  return response.data;
};

const deleteAll = async () => {
  const response = await api.delete('/personal-property-securities');
  return response.data;
};

export default personalPropertySecuritiesApi;
