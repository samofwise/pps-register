import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import PersonalPropertySecurityUpload from "../models/PersonalPropertySecurityUploadResponse";
import api from "./axiosConfig";
import delay from "delay";
import PersonalPropertySecurity from "../models/PersonalPropertySecurity";


const uploadsKey = 'personalPropertySecurityUploads'
const personalPropertySecuritiesKey = 'personalPropertySecurities'
const personalPropertySecuritiesApi = {
  useGetUploads: () => useQuery({
    queryKey: [uploadsKey],
    queryFn: getUploads
  }),

  useInvalidateUploads: () => {
    const queryClient = useQueryClient()
    return () => queryClient.invalidateQueries({ queryKey: [uploadsKey] })
  },

  useGetPersonalPropertySecurities: () => useQuery({
    queryKey: [personalPropertySecuritiesKey],
    queryFn: getPersonalPropertySecurities
  }),

  useInvalidatePersonalPropertySecurities: () => {
    const queryClient = useQueryClient()
    return () => queryClient.invalidateQueries({ queryKey: [personalPropertySecuritiesKey] })
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

const getPersonalPropertySecurities = async () => {
  const promise = api.get<PersonalPropertySecurity[]>('/personal-property-securities');
  const [response] = await Promise.all([promise, delay(300)]);
  return response.data;
};

const uploadFile = async (file: File) => {
  const formData = new FormData();
  formData.append('file', file);
  const promise = api.post('/personal-property-securities', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
  const [response] = await Promise.all([promise, delay(300)]);
  return response.data;
};

const deleteAll = async () => {
  const response = await api.delete('/personal-property-securities');
  return response.data;
};

export default personalPropertySecuritiesApi;
