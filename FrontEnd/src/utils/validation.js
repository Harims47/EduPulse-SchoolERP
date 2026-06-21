/**
 * Custom validation helper functions for EduPulse Form validation.
 */

export const required = (value) => {
  if (value === undefined || value === null || value === '') {
    return 'This field is required.';
  }
  return null;
};

export const email = (value) => {
  if (!value) return null;
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(value)) {
    return 'Please enter a valid email address.';
  }
  return null;
};

export const phone = (value) => {
  if (!value) return null;
  // Simple phone check: digits, spaces, hyphens, plus sign, min 7 max 15 digits
  const phoneRegex = /^\+?[0-9\s\-]{7,15}$/;
  if (!phoneRegex.test(value)) {
    return 'Please enter a valid phone number (7-15 digits).';
  }
  return null;
};

export const minLength = (value, len) => {
  if (!value) return null;
  if (value.trim().length < len) {
    return `Minimum length is ${len} characters.`;
  }
  return null;
};

export const maxLength = (value, len) => {
  if (!value) return null;
  if (value.trim().length > len) {
    return `Maximum length is ${len} characters.`;
  }
  return null;
};

export const numberRange = (value, min, max) => {
  const num = Number(value);
  if (isNaN(num)) {
    return 'Must be a valid number.';
  }
  if (num < min || num > max) {
    return `Must be between ${min} and ${max}.`;
  }
  return null;
};

export const dateBeforeOrToday = (value) => {
  if (!value) return null;
  const selectedDate = new Date(value);
  const today = new Date();
  // Clear time components for date-only comparison
  selectedDate.setHours(0,0,0,0);
  today.setHours(0,0,0,0);
  
  if (selectedDate > today) {
    return 'Date cannot be in the future.';
  }
  return null;
};

export const validateForm = (data, rules) => {
  const errors = {};
  let isValid = true;

  Object.keys(rules).forEach((field) => {
    const value = data[field];
    const fieldRules = rules[field];

    for (let rule of fieldRules) {
      const errorMsg = rule(value);
      if (errorMsg) {
        errors[field] = errorMsg;
        isValid = false;
        break; // Stop evaluating further rules for this field
      }
    }
  });

  return { isValid, errors };
};
