import { createSlice } from '@reduxjs/toolkit';

const academicContextSlice = createSlice({
  name: 'academicContext',
  initialState: {
    activeAcademicYearId: localStorage.getItem('ep_active_year_id') || '',
    activeYearLabel: localStorage.getItem('ep_active_year_label') || 'Select Year',
    availableYears: []
  },
  reducers: {
    setActiveYear: (state, action) => {
      state.activeAcademicYearId = action.payload.id;
      state.activeYearLabel = action.payload.label;
      localStorage.setItem('ep_active_year_id', action.payload.id);
      localStorage.setItem('ep_active_year_label', action.payload.label);
    },
    setAvailableYears: (state, action) => {
      state.availableYears = action.payload;
      // Auto-select first year if none active or active not in available
      if (action.payload.length > 0) {
        const found = action.payload.find(y => y.academicYearId === state.activeAcademicYearId);
        if (!found) {
          state.activeAcademicYearId = action.payload[0].academicYearId;
          state.activeYearLabel = action.payload[0].name;
          localStorage.setItem('ep_active_year_id', action.payload[0].academicYearId);
          localStorage.setItem('ep_active_year_label', action.payload[0].name);
        }
      }
    }
  }
});

export const { setActiveYear, setAvailableYears } = academicContextSlice.actions;
export default academicContextSlice.reducer;
