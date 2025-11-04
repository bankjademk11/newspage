import React from 'react';
import { useTranslation } from 'react-i18next';
import { NEWS_CATEGORIES } from '../services/newsApi';

interface CategoryNavProps {
  selectedCategory?: string;
  onCategoryChange: (category: string | undefined) => void;
}

export default function CategoryNav({ selectedCategory, onCategoryChange }: CategoryNavProps) {
  const { t, i18n } = useTranslation();

  const getCategoryLabel = (category: any) => {
    // Use translation key for category labels
    const translationKey = `categories.${category.value}`;
    return t(translationKey) as string || category.label;
  };

  return (
    <nav className="apple-nav">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center space-x-8">
            <button
              onClick={() => onCategoryChange(undefined)}
              className={`px-4 py-2 rounded-xl text-sm font-medium transition-all duration-200 ${
                !selectedCategory
                  ? 'bg-blue-100 text-blue-700 shadow-sm'
                  : 'text-gray-600 hover:text-gray-900 hover:bg-gray-100'
              }`}
            >
              {t('header.home')}
            </button>
            
            <div className="hidden md:flex items-center space-x-2">
              {NEWS_CATEGORIES.map((category) => (
                <button
                  key={category.value}
                  onClick={() => onCategoryChange(category.value)}
                  className={`px-4 py-2 rounded-xl text-sm font-medium transition-all duration-200 ${
                    selectedCategory === category.value
                      ? 'bg-blue-100 text-blue-700 shadow-sm'
                      : 'text-gray-600 hover:text-gray-900 hover:bg-gray-100'
                  }`}
                >
                  {getCategoryLabel(category)}
                </button>
              ))}
            </div>
          </div>
          
          {/* Mobile menu button */}
          <div className="md:hidden">
            <button className="text-gray-600 hover:text-gray-900 p-2 rounded-xl hover:bg-gray-100 transition-all duration-200">
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>
        
        {/* Mobile menu */}
        <div className="md:hidden border-t border-gray-200/50 py-3">
          <div className="flex flex-wrap gap-2">
            {NEWS_CATEGORIES.map((category) => (
              <button
                key={category.value}
                onClick={() => onCategoryChange(category.value)}
                className={`px-3 py-1.5 rounded-full text-xs font-medium transition-all duration-200 ${
                  selectedCategory === category.value
                    ? 'bg-blue-100 text-blue-700 shadow-sm'
                    : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                }`}
              >
                {getCategoryLabel(category)}
              </button>
            ))}
          </div>
        </div>
      </div>
    </nav>
  );
}
